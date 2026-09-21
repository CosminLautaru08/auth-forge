using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public interface IRegistrationRepository
{
    Task AddAsync
        (
            User user,
            PasswordCredential passwordCredential,
            CancellationToken cancelationToken = default
        );



}