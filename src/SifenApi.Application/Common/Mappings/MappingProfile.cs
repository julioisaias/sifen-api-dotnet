using AutoMapper;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Response;
using SifenApi.Application.DTOs.Events;
using SifenApi.Application.Eventos.Commands;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;

namespace SifenApi.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // DocumentoElectronico mappings
        CreateMap<DocumentoElectronico, DocumentoElectronicoResponse>()
            .ForMember(dest => dest.TipoDocumentoDescripcion, 
                opt => opt.MapFrom(src => GetTipoDocumentoDescripcion(src.TipoDocumento)))
            .ForMember(dest => dest.Cliente, 
                opt => opt.MapFrom(src => src.Cliente))
            .ForMember(dest => dest.Estado, 
                opt => opt.MapFrom(src => src.Estado.ToString()));

        CreateMap<Cliente, ClienteResponseDto>()
            .ForMember(dest => dest.Ruc, 
                opt => opt.MapFrom(src => src.Ruc != null ? src.Ruc.Completo : null))
            .ForMember(dest => dest.Nombre, 
                opt => opt.MapFrom(src => src.Nombre ?? src.RazonSocial));

        CreateMap<EventoDocumento, EventoResponse>()
            .ForMember(dest => dest.TipoEvento, 
                opt => opt.MapFrom(src => src.TipoEvento.ToString()))
            .ForMember(dest => dest.Estado, 
                opt => opt.MapFrom(src => src.Estado.ToString()));

        // Domain to DTOs
        CreateMap<Contribuyente, ContribuyenteDto>()
            .ForMember(dest => dest.Ruc, 
                opt => opt.MapFrom(src => src.Ruc.Completo));

        CreateMap<ActividadEconomica, ActividadEconomicaDto>();
        
        // Simple event DTOs mappings
        CreateMap<CancelacionDto, EventoDocumento>()
            .ForMember(dest => dest.Motivo, opt => opt.MapFrom(src => src.Motivo));

        CreateMap<InutilizacionDto, EventoDocumento>()
            .ForMember(dest => dest.Motivo, opt => opt.MapFrom(src => src.Motivo));
    }

    private static string GetTipoDocumentoDescripcion(TipoDocumento tipo)
    {
        return tipo switch
        {
            TipoDocumento.FacturaElectronica => "Factura electrónica",
            TipoDocumento.FacturaElectronicaExportacion => "Factura electrónica de exportación",
            TipoDocumento.FacturaElectronicaImportacion => "Factura electrónica de importación",
            TipoDocumento.AutofacturaElectronica => "Autofactura electrónica",
            TipoDocumento.NotaCreditoElectronica => "Nota de crédito electrónica",
            TipoDocumento.NotaDebitoElectronica => "Nota de débito electrónica",
            TipoDocumento.NotaRemisionElectronica => "Nota de remisión electrónica",
            TipoDocumento.ComprobanteRetencionElectronico => "Comprobante de retención electrónico",
            _ => "Documento electrónico"
        };
    }
}
