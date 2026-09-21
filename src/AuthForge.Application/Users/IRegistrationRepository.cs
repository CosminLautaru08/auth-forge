using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public interface IRegistrationRepository
{
    Task<bool> ExistsByEmailAsync
       (
           string email,
           CancellationToken cancelationToken = default
       );
    Task AddAsync
        (
            User user,
            PasswordCredential passwordCredential,
            CancellationToken cancelationToken = default
        );

}