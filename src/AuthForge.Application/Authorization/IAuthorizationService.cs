using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.Application.Authorization;

public interface IAuthorizationService
{
    bool HasPermission(
       User user,
       Permission permission);
}