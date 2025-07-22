namespace SifenApi.WebApi.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        // Agregar middleware personalizado aquí si es necesario
        return app;
    }
}