using AuthForge.Application.Security;
using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public class RegisterUser
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRegistrationRepository _registrationRepository;

    public RegisterUser(IPasswordHasher passwordHasher, IRegistrationRepository registrationRepository)
    {
        _passwordHasher = passwordHasher;
        _registrationRepository = registrationRepository;
    }

    public async Task<RegistrationResult> ExecuteAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var user = new User(email);

        var passwordHash = _passwordHasher.Hash(password);

        var passwordCredential = new PasswordCredential(
            user.Id,
            passwordHash
        );

        await _registrationRepository.AddAsync(
            user,
            passwordCredential,
            cancellationToken
        );

        return new RegistrationResult(
            user,
            passwordCredential
        );
    }

}