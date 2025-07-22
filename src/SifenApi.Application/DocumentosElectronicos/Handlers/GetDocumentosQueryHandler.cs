using AutoMapper;
using MediatR;
using SifenApi.Application.Common.Interfaces;
using SifenApi.Application.DocumentosElectronicos.Queries;
using SifenApi.Application.DTOs.Response;
using SifenApi.Domain.Interfaces;
using SifenApi.Domain.ValueObjects;

namespace SifenApi.Application.DocumentosElectronicos.Handlers;

public class GetDocumentosQueryHandler : IRequestHandler<GetDocumentosQuery, ApiResponse<PagedResponse<DocumentoElectronicoResponse>>>
{
    private readonly IDocumentoElectronicoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetDocumentosQueryHandler(
        IDocumentoElectronicoRepository repository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<PagedResponse<DocumentoElectronicoResponse>>> Handle(
        GetDocumentosQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contribuyenteId = request.ContribuyenteId != Guid.Empty 
                ? request.ContribuyenteId 
                : _currentUserService.ContribuyenteId;

            var allDocumentos = await _repository.GetByContribuyenteAsync(
                contribuyenteId,
                cancellationToken);

            var documentosList = allDocumentos.ToList();
            var totalCount = documentosList.Count;

            // Paginación en memoria (TODO: Mover a repositorio para mejor rendimiento)
            var pagedDocumentos = documentosList
                .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
                .Take(request.Request.PageSize)
                .ToList();

            var response = _mapper.Map<List<DocumentoElectronicoResponse>>(pagedDocumentos);

            var pagedResponse = new PagedResponse<DocumentoElectronicoResponse>
            {
                Items = response,
                PageNumber = request.Request.PageNumber,
                PageSize = request.Request.PageSize,
                TotalCount = totalCount
            };

            return ApiResponse<PagedResponse<DocumentoElectronicoResponse>>.Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResponse<DocumentoElectronicoResponse>>.Fail(
                $"Error al obtener documentos: {ex.Message}");
        }
    }
}