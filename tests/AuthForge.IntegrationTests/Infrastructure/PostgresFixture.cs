using AuthForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace AuthForge.IntegrationTests.Infrastructure;

public sealed class PostgresFixture : IAsyncLifetime
{

    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:15.1")
            .Build();
    public string ConnectionString => _container.GetConnectionString();


    public AuthForgeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AuthForgeDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new AuthForgeDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var dbContext = CreateDbContext();

        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}