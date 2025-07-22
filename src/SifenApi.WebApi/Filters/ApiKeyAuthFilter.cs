using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace SifenApi.WebApi.Filters;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Skip si ya está autenticado con JWT
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            return;

        // Skip para endpoints que permiten acceso anónimo
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
            return;

        // Verificar API Key
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "API Key no proporcionada"
            });
            return;
        }

        var validApiKeys = _configuration.GetSection("ApiKeys:ValidKeys").Get<string[]>() ?? Array.Empty<string>();
        
        if (!validApiKeys.Contains(extractedApiKey.ToString()))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "API Key inválida"
            });
            return;
        }
    }
}