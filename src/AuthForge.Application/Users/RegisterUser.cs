using AuthForge.Application.Security;
using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public class RegisterUser
{
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUser(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public User Execute(string email)
    {
        return new User(email);
    }
}