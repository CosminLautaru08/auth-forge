using AuthForge.Application.Security;
using AuthForge.Domain.Entities;
using AuthForge.Application.Users;

namespace AuthForge.Application.Users;

public class RegisterUser
{
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUser(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public RegistrationResult Execute(string email, string password)
    {
        var user = new User(email);

        var passwordHash = _passwordHasher.Hash(password);

        var passwordCredential = new PasswordCredential(
            user.Id,
            passwordHash);

        return new RegistrationResult(
            user,
            passwordCredential);
    }
}