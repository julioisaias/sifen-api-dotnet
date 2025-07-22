using MediatR;
using Microsoft.Extensions.Logging;
using SifenApi.Application.Common.Interfaces;
using SifenApi.Application.DocumentosElectronicos.Commands;
using SifenApi.Application.DTOs.Response;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;
using SifenApi.Domain.Interfaces;
using SifenApi.Domain.ValueObjects;
using AutoMapper;

namespace SifenApi.Application.DocumentosElectronicos.Handlers;

public class CreateFacturaHandler : IRequestHandler<CreateFacturaCommand, ApiResponse<DocumentoElectronicoResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IXmlGenerator _xmlGenerator;
    private readonly IXmlSigner _xmlSigner;
    private readonly IQrGenerator _qrGenerator;
    private readonly ISifenClient _sifenClient;
    private readonly IKudeGenerator _kudeGenerator;
    private readonly IFileStorageService _fileStorage;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFacturaHandler> _logger;

    public CreateFacturaHandler(
        IUnitOfWork unitOfWork,
        IXmlGenerator xmlGenerator,
        IXmlSigner xmlSigner,
        IQrGenerator qrGenerator,
        ISifenClient sifenClient,
        IKudeGenerator kudeGenerator,
        IFileStorageService fileStorage,
        IMapper mapper,
        ILogger<CreateFacturaHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _xmlGenerator = xmlGenerator;
        _xmlSigner = xmlSigner;
        _qrGenerator = qrGenerator;
        _sifenClient = sifenClient;
        _kudeGenerator = kudeGenerator;
        _fileStorage = fileStorage;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<DocumentoElectronicoResponse>> Handle(
        CreateFacturaCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // 1. Obtener datos del contribuyente
            var contribuyente = await _unitOfWork.Contribuyentes
                .GetByIdWithEstablecimientosAsync(request.ContribuyenteId, cancellationToken);
            
            if (contribuyente == null)
                return ApiResponse<DocumentoElectronicoResponse>.Fail("Contribuyente no encontrado");

            // 2. Obtener timbrado vigente
            var timbrado = await _unitOfWork.Timbrados
                .GetVigenteByContribuyenteAsync(request.ContribuyenteId, cancellationToken);
            
            if (timbrado == null || !timbrado.EstaVigente())
                return ApiResponse<DocumentoElectronicoResponse>.Fail("No existe timbrado vigente");

            // 3. Obtener número de documento
            var numeroDocumento = request.Factura.Numero;
            if (string.IsNullOrEmpty(numeroDocumento))
            {
                numeroDocumento = await _unitOfWork.DocumentosElectronicos.GetProximoNumeroAsync(
                    request.Factura.Establecimiento,
                    request.Factura.PuntoExpedicion,
                    (TipoDocumento)request.Factura.TipoDocumento,
                    cancellationToken);
            }
            else
            {
                // Verificar que no exista
                var existe = await _unitOfWork.DocumentosElectronicos.ExisteNumeroDocumentoAsync(
                    request.Factura.Establecimiento,
                    request.Factura.PuntoExpedicion,
                    numeroDocumento,
                    cancellationToken);
                
                if (existe)
                    return ApiResponse<DocumentoElectronicoResponse>.Fail(
                        $"Ya existe un documento con el número {numeroDocumento}");
            }

            // 4. Crear documento electrónico
            var documento = new DocumentoElectronico(
                (TipoDocumento)request.Factura.TipoDocumento,
                request.Factura.Establecimiento,
                request.Factura.PuntoExpedicion,
                numeroDocumento,
                request.Factura.Fecha,
                (TipoEmision)request.Factura.TipoEmision,
                (TipoTransaccion)request.Factura.TipoTransaccion,
                timbrado.Id,
                contribuyente.Id,
                request.Factura.Moneda,
                (CondicionVenta)request.Factura.Condicion.Tipo);

            documento.SetCreatedBy(request.UserId);

            // 5. Gestionar cliente
            Cliente? cliente = null;
            if (request.Factura.Cliente.Contribuyente && !string.IsNullOrEmpty(request.Factura.Cliente.Ruc))
            {
                cliente = await _unitOfWork.Clientes.GetByRucAsync(
                    request.Factura.Cliente.Ruc, 
                    cancellationToken);
                
                if (cliente == null)
                {
                    cliente = Cliente.CrearContribuyente(
                        request.Factura.Cliente.Ruc,
                        request.Factura.Cliente.RazonSocial ?? string.Empty,
                        request.Factura.Cliente.Direccion,
                        request.Factura.Cliente.Telefono,
                        request.Factura.Cliente.Email);
                    
                    await _unitOfWork.Clientes.AddAsync(cliente, cancellationToken);
                }
            }
            else if (!request.Factura.Cliente.Contribuyente && 
                     request.Factura.Cliente.DocumentoTipo.HasValue &&
                     !string.IsNullOrEmpty(request.Factura.Cliente.DocumentoNumero))
            {
                cliente = await _unitOfWork.Clientes.GetByDocumentoAsync(
                    request.Factura.Cliente.DocumentoTipo.Value,
                    request.Factura.Cliente.DocumentoNumero,
                    cancellationToken);
                
                if (cliente == null)
                {
                    cliente = Cliente.CrearNoContribuyente(
                        (TipoDocumentoIdentidad)request.Factura.Cliente.DocumentoTipo.Value,
                        request.Factura.Cliente.DocumentoNumero,
                        request.Factura.Cliente.RazonSocial ?? request.Factura.Cliente.NombreFantasia ?? "Sin nombre",
                        request.Factura.Cliente.Direccion,
                        request.Factura.Cliente.Telefono,
                        request.Factura.Cliente.Email);
                    
                    await _unitOfWork.Clientes.AddAsync(cliente, cancellationToken);
                }
            }

            if (cliente != null)
                documento.AsignarCliente(cliente);

            // 6. Agregar items
            foreach (var itemDto in request.Factura.Items)
            {
                var item = new Item(
                    itemDto.Codigo,
                    itemDto.Descripcion,
                    itemDto.UnidadMedida,
                    itemDto.Cantidad,
                    itemDto.PrecioUnitario,
                    (TipoIva)itemDto.IvaTipo,
                    itemDto.Iva);

                if (itemDto.Descuento.HasValue && itemDto.Descuento.Value > 0)
                    item.AplicarDescuento(itemDto.Descuento.Value);

                documento.AgregarItem(item);
            }

            // 7. Generar CDC
            documento.GenerarCdc(contribuyente);

            // 8. Guardar documento
            await _unitOfWork.DocumentosElectronicos.AddAsync(documento, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Generar XML
            var parametros = MapearParametrosContribuyente(contribuyente, timbrado);
            var xml = await _xmlGenerator.GenerateDocumentoElectronicoXmlAsync(
                documento, 
                parametros, 
                cancellationToken);

            // 10. Firmar XML
            var xmlFirmado = await _xmlSigner.SignXmlWithStoredCertificateAsync(
                xml, 
                contribuyente.Id, 
                cancellationToken);
            
            documento.EstablecerXmlFirmado(xmlFirmado);

            // 11. Generar QR
            var qrData = await _qrGenerator.GenerateQrAsync(
                documento.Cdc!.Value,
                documento.Total,
                cliente?.Ruc?.Completo,
                null,
                cancellationToken);
            
            documento.EstablecerQr(qrData);

            // 12. Enviar a SIFEN (si no es contingencia)
            if (request.Factura.TipoEmision == (int)TipoEmision.Normal)
            {
                var ambiente = "test"; // TODO: Obtener de configuración
                var sifenResponse = await _sifenClient.RecibeAsync(
                    xmlFirmado,
                    ambiente,
                    string.Empty, // Path se obtiene internamente
                    string.Empty, // Password se obtiene internamente
                    cancellationToken);

                if (sifenResponse.Success)
                {
                    documento.AprobarDocumento(
                        sifenResponse.NumeroControl ?? string.Empty,
                        sifenResponse.Protocolo ?? string.Empty);
                }
                else
                {
                    documento.RechazarDocumento(
                        sifenResponse.Mensaje ?? "Error desconocido");
                }
            }

            // 13. Generar KUDE
            var kudePdf = await _kudeGenerator.GenerateKudePdfAsync(
                xmlFirmado, 
                qrData, 
                cancellationToken);
            
            var kudeFileName = $"KUDE_{documento.Cdc!.Value}.pdf";
            var kudeUrl = await _fileStorage.SaveFileAsync(
                kudePdf,
                kudeFileName,
                "application/pdf",
                "kudes",
                cancellationToken);

            // 14. Guardar XML
            var xmlFileName = $"DE_{documento.Cdc!.Value}.xml";
            var xmlUrl = await _fileStorage.SaveFileAsync(
                System.Text.Encoding.UTF8.GetBytes(xmlFirmado),
                xmlFileName,
                "application/xml",
                "xml",
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // 15. Mapear respuesta
            var response = _mapper.Map<DocumentoElectronicoResponse>(documento);
            response.KudeUrl = kudeUrl;
            response.XmlUrl = xmlUrl;

            _logger.LogInformation(
                "Documento electrónico creado exitosamente. CDC: {Cdc}, Estado: {Estado}",
                documento.Cdc!.Value,
                documento.Estado);

            return ApiResponse<DocumentoElectronicoResponse>.Ok(
                response,
                "Documento electrónico creado exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error al crear documento electrónico");
            return ApiResponse<DocumentoElectronicoResponse>.Fail(
                "Error al crear documento electrónico",
                ex.Message);
        }
    }

    private ContribuyenteParams MapearParametrosContribuyente(Contribuyente contribuyente, Timbrado timbrado)
    {
        return new ContribuyenteParams
        {
            Ruc = contribuyente.Ruc.Completo,
            RazonSocial = contribuyente.RazonSocial,
            NombreFantasia = contribuyente.NombreFantasia,
            TipoContribuyente = (int)contribuyente.TipoContribuyente,
            TipoRegimen = contribuyente.TipoRegimen,
            TimbradoNumero = timbrado.Numero,
            TimbradoFecha = timbrado.FechaInicio,
            ActividadesEconomicas = contribuyente.ActividadesEconomicas
                .Select(a => new ActividadEconomicaParams
                {
                    Codigo = a.Codigo,
                    Descripcion = a.Descripcion
                })
                .ToList(),
            Establecimientos = contribuyente.Establecimientos
                .Select(e => new EstablecimientoParams
                {
                    Codigo = e.Codigo,
                    Denominacion = e.Denominacion,
                    Direccion = e.Direccion,
                    NumeroCasa = e.NumeroCasa,
                    ComplementoDireccion1 = e.ComplementoDireccion1,
                    ComplementoDireccion2 = e.ComplementoDireccion2,
                    Departamento = e.Departamento,
                    DepartamentoDescripcion = e.DepartamentoDescripcion,
                    Distrito = e.Distrito,
                    DistritoDescripcion = e.DistritoDescripcion,
                    Ciudad = e.Ciudad,
                    CiudadDescripcion = e.CiudadDescripcion,
                    Telefono = e.Telefono,
                    Email = e.Email
                })
                .ToList()
        };
    }
}