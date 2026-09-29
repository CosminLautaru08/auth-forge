using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AuthForge.Infrastructure.Persistence;

namespace AuthForge.IntegrationTests.Infrastructure;

public class AuthForgeWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public AuthForgeWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                service =>
                    service.ServiceType ==
                    typeof(DbContextOptions<AuthForgeDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AuthForgeDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });
        });
    }
}