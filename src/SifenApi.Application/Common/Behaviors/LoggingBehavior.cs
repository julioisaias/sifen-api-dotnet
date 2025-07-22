using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SifenApi.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Procesando request: {Name} {@UserId} {@Request}",
            requestName, requestGuid, request);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "Request completado: {Name} {ElapsedMilliseconds}ms {@UserId}",
                requestName, stopwatch.ElapsedMilliseconds, requestGuid);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "Request falló: {Name} {ElapsedMilliseconds}ms {@UserId}",
                requestName, stopwatch.ElapsedMilliseconds, requestGuid);

            throw;
        }
    }
}