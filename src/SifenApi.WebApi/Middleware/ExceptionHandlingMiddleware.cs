using System.Net;
using System.Text.Json;
using SifenApi.Application.Common.Exceptions;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ha ocurrido un error no manejado");

        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = new ApiResponse<object>();
        apiResponse.Success = false;

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResponse.Message = "Error de validación";
                apiResponse.Errors = validationException.Errors
                    .SelectMany(e => e.Value)
                    .ToList();
                break;

            case NotFoundException notFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResponse.Message = notFoundException.Message;
                break;

            case SifenException sifenException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResponse.Message = sifenException.Message;
                apiResponse.Errors = sifenException.Detalles;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiResponse.Message = "No autorizado";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = "Ha ocurrido un error en el servidor";
                
                if (_environment.IsDevelopment())
                {
                    apiResponse.Errors = new List<string> 
                    { 
                        exception.Message,
                        exception.StackTrace ?? ""
                    };
                }
                break;
        }

        var jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };
        
        var result = JsonSerializer.Serialize(apiResponse, jsonOptions);
        await response.WriteAsync(result);
    }
}