using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public class RegistrationResult
{
    public User User { get; }

    public PasswordCredential PasswordCredential { get; }

    public RegistrationResult(User user, PasswordCredential passwordCredential)
    {
        User = user;
        PasswordCredential = passwordCredential;
    }
}