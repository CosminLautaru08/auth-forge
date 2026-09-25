using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}