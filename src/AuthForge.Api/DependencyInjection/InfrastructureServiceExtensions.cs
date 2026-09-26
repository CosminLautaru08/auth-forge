using AuthForge.Application.Security;
using AuthForge.Application.Sessions;
using AuthForge.Application.Users;
using AuthForge.Infrastructure.Persistence;
using AuthForge.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthForge.Api.DependencyInjection;

public static class InfrastructureServiceExtensions
{

    public static IServiceCollection AddInfrastructure(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<ILoginRepository, LoginRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddDbContext<AuthForgeDbContext>(options =>
       {
           options.UseNpgsql(
               configuration.GetConnectionString("AuthForge"));
       });

        return services;
    }
}