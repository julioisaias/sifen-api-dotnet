using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SifenApi.Infrastructure;
using SifenApi.Infrastructure.Persistence.Context;
using SifenApi.Application.Common.Interfaces;
using Xunit;

namespace SifenApi.Test.Integration;

public class TestCurrentUserService : ICurrentUserService
{
    public string UserId => "test-user-id";
    public string UserName => "testuser@test.com";
    public Guid ContribuyenteId => Guid.NewGuid();
    public bool IsAuthenticated => true;
    public List<string> Roles => new() { "Admin", "User" };
}

public class DatabaseTest
{
    [Fact]
    public async Task CanConfigureSqliteForTesting()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", "Data Source=:memory:"},
                {"Jwt:Key", "TuClaveSecretaSuperSeguraDeAlMenos256BitsParaProduccion"},
                {"Jwt:Issuer", "SifenApi"},
                {"Jwt:Audience", "SifenApiUsers"},
                {"ConnectionStrings:Redis", "localhost:6379"}
            })
            .Build();

        var services = new ServiceCollection();
        services.AddScoped<ICurrentUserService>(_ => new TestCurrentUserService());
        services.AddInfrastructure(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SifenDbContext>();

        // Verificar que se puede crear la base de datos
        await context.Database.EnsureCreatedAsync();

        // Assert
        Assert.True(context.Database.IsSqlite());
    }

    [Fact]
    public async Task CanUseSqlServerForProduction()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDb;Trusted_Connection=true;"},
                {"Jwt:Key", "TuClaveSecretaSuperSeguraDeAlMenos256BitsParaProduccion"},
                {"Jwt:Issuer", "SifenApi"},
                {"Jwt:Audience", "SifenApiUsers"},
                {"ConnectionStrings:Redis", "localhost:6379"}
            })
            .Build();

        var services = new ServiceCollection();
        services.AddScoped<ICurrentUserService>(_ => new TestCurrentUserService());
        services.AddInfrastructure(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SifenDbContext>();

        // Assert
        Assert.True(context.Database.IsSqlServer());
    }
}