namespace SifenApi.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserName { get; }
    Guid ContribuyenteId { get; }
    bool IsAuthenticated { get; }
    List<string> Roles { get; }
}