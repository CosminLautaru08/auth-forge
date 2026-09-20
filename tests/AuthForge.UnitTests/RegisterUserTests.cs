using AuthForge.Application.Security;
using AuthForge.Application.Users;
using AuthForge.Domain.Enums;

namespace AuthForge.UnitTests.Users;

public class RegisterUserTests
{
    [Fact]
    public void Execute_WithValidEmail_CreatesActiveUser()
    {
        var registerUser = new RegisterUser(new FakePasswordHasher());

        var result = registerUser.Execute("user@example.com", "password123");

        Assert.Equal(
            result.User.Id,
            result.PasswordCredential.UserId);
        Assert.Equal(
        "hashed:password123",
        result.PasswordCredential.PasswordHash);
    }

    [Fact]
    public void Execute_WithEmptyEmail_ThrowsArgumentException()
    {
        var registerUser = new RegisterUser(new FakePasswordHasher());

        Assert.Throws<ArgumentException>(() =>
             registerUser.Execute("", "password123"));
    }

    private class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }
}

