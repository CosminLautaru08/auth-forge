using AuthForge.Application.Security;
using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;


namespace AuthForge.Application.Users;


public class LoginUser
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILoginRepository _loginRepository;

    public LoginUser(
        IPasswordHasher passwordHasher,
        ILoginRepository loginRepository
    )
    {
        _passwordHasher = passwordHasher;
        _loginRepository = loginRepository;
    }

    public async Task<User> ExecuteAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _loginRepository.FindByEmailAsync(
            email,
            cancellationToken
        );

        if (result is null)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var (user, passwordCredential) = result.Value;

        if (user.Status == UserStatus.Suspended)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var passwordValid = _passwordHasher.Verify(
            password,
            passwordCredential.PasswordHash
        );

        if (!passwordValid)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        return user;
    }
}