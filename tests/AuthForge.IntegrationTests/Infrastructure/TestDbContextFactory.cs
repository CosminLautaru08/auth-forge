using AuthForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthForge.IntegrationTests.Infrastructure;

public static class TestDbContextFactory
{
    public static AuthForgeDbContext Create()
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "AUTHFORGE_TEST_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "AUTHFORGE_TEST_CONNECTION_STRING is not configured.");
        }

        var options = new DbContextOptionsBuilder<AuthForgeDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AuthForgeDbContext(options);
    }
}