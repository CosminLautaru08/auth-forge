using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.Application.Authorization;

public class AuthorizationService : IAuthorizationService
{
    public bool HasPermission(
         User user,
         Permission permission)
    {
        return user.Role switch
        {
            UserRole.Admin => true,
            _ => false
        };
    }
}