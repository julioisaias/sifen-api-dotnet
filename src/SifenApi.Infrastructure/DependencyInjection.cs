using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SifenApi.Application.Common.Interfaces;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence;
using SifenApi.Infrastructure.Persistence.Context;
using SifenApi.Infrastructure.Persistence.Repositories;
using SifenApi.Infrastructure.Security;
using SifenApi.Infrastructure.Services;
using SifenApi.Infrastructure.Services.Kude;
using SifenApi.Infrastructure.Services.Qr;
using SifenApi.Infrastructure.Services.Sifen;
using SifenApi.Infrastructure.Services.Xml;

namespace SifenApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<SifenDbContext>(options =>
        {
            if (connectionString?.Contains(":memory:") == true || connectionString?.Contains(".db") == true)
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly(typeof(SifenDbContext).Assembly.FullName));
            }
        });

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDocumentoElectronicoRepository, DocumentoElectronicoRepository>();
        services.AddScoped<IContribuyenteRepository, ContribuyenteRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ITimbradoRepository, TimbradoRepository>();
        services.AddScoped<IEventoDocumentoRepository, EventoDocumentoRepository>();
        services.AddScoped<IEventStore, EventStore>();

        // Services
        services.AddScoped<IXmlGenerator, XmlGenerator>();
        services.AddScoped<IXmlSigner, XmlSigner>();
        services.AddScoped<IQrGenerator, QrGenerator>();
        services.AddScoped<IKudeGenerator, KudeGenerator>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IDateTime, DateTimeService>();
        
        // Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            options.InstanceName = "SifenApi";
        });
        services.AddScoped<ICacheService, CacheService>();

        // SIFEN Client
        services.AddHttpClient<ISifenClient, SifenClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/soap+xml");
        });

        // Security
        services.AddScoped<CertificateManager>();

        return services;
    }
}