using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public interface ILoginRepository
{
    Task<(User user, PasswordCredential passwordCredential)?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );
}