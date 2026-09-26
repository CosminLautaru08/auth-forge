using AuthForge.Application.Authorization;
using AuthForge.Application.Sessions;
using AuthForge.Application.Users;

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
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<PromoteUserToAdmin>();


        return services;
    }
}