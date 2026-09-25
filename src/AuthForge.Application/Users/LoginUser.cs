using AuthForge.Application.Security;
using AuthForge.Application.Sessions;
using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;


namespace AuthForge.Application.Users;


public class LoginUser
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILoginRepository _loginRepository;

    private readonly ICreateSession _createSession;

    public LoginUser(
        IPasswordHasher passwordHasher,
        ILoginRepository loginRepository,
        ICreateSession createSession
    )
    {
        _passwordHasher = passwordHasher;
        _loginRepository = loginRepository;
        _createSession = createSession;
    }

    public async Task<UserSession> ExecuteAsync(
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

        var session = await _createSession.ExecuteAsync(
            user.Id,
            cancellationToken
        );

        return session;
    }
}