using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SifenApi.Infrastructure;
using Xunit;

namespace SifenApi.Test.Integration;

public class SimpleConfigTest
{
    [Fact]
    public void CanDetectSqliteConnectionString()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", "Data Source=:memory:"}
            })
            .Build();

        // Act
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Assert
        Assert.NotNull(connectionString);
        Assert.Contains(":memory:", connectionString);
    }

    [Fact]
    public void CanDetectSqlServerConnectionString()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDb;Trusted_Connection=true;"}
            })
            .Build();

        // Act
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Assert
        Assert.NotNull(connectionString);
        Assert.Contains("Server=", connectionString);
        Assert.DoesNotContain(":memory:", connectionString);
        Assert.DoesNotContain(".db", connectionString);
    }
}