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

        var user = registerUser.Execute("user@example.com");

        Assert.Equal("user@example.com", user.Email);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public void Execute_WithEmptyEmail_ThrowsArgumentException()
    {
        var registerUser = new RegisterUser(new FakePasswordHasher());

        Assert.Throws<ArgumentException>(() =>
            registerUser.Execute("")
        );
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

