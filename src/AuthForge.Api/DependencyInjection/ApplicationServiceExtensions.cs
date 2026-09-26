using AuthForge.Application.Security;
using AuthForge.Application.Sessions;
using AuthForge.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace AuthForge.Api.DependencyInjection;

public static class ApplicationServiceExtentions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services
    )
    {
        services.AddScoped<RegisterUser>();
        services.AddScoped<LoginUser>();
        services.AddScoped<ICreateSession, CreateSession>();
        services.AddScoped<ValidateSession>();

        return services;
    }
}