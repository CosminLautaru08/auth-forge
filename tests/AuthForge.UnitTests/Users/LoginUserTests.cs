using AuthForge.Application.Security;
using AuthForge.Application.Users;
using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.UnitTests.Users;

public class LoginUserTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsUser()
    {
        var user = new User("user@example.com");

        var credential = new PasswordCredential(
            user.Id,
            "hashed:password123");

        var repository = new FakeLoginRepository
        {
            User = user,
            PasswordCredential = credential
        };

        var loginUser = new LoginUser(
            new FakePasswordHasher(),
            repository);

        var result = await loginUser.ExecuteAsync(
            "user@example.com",
            "password123");

        Assert.Same(user, result);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownEmail_ThrowsInvalidOperationException()
    {
        var loginUser = new LoginUser(
            new FakePasswordHasher(),
            new FakeLoginRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            loginUser.ExecuteAsync(
                "unknown@example.com",
                "password123"));

        Assert.Equal(
            "Invalid email or password.",
            exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPassword_ThrowsInvalidOperationException()
    {
        var user = new User("user@example.com");

        var credential = new PasswordCredential(
            user.Id,
            "hashed:correct-password");

        var repository = new FakeLoginRepository
        {
            User = user,
            PasswordCredential = credential
        };

        var loginUser = new LoginUser(
            new FakePasswordHasher(),
            repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            loginUser.ExecuteAsync(
                "user@example.com",
                "wrong-password"));

        Assert.Equal(
            "Invalid email or password.",
            exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithSuspendedUser_ThrowsInvalidOperationException()
    {
        var user = new User("user@example.com");
        user.Suspend();

        var credential = new PasswordCredential(
            user.Id,
            "hashed:password123");

        var repository = new FakeLoginRepository
        {
            User = user,
            PasswordCredential = credential
        };

        var loginUser = new LoginUser(
            new FakePasswordHasher(),
            repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            loginUser.ExecuteAsync(
                "user@example.com",
                "password123"));

        Assert.Equal(
            "Invalid email or password.",
            exception.Message);
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

    private class FakeLoginRepository : ILoginRepository
    {
        public User? User { get; set; }

        public PasswordCredential? PasswordCredential { get; set; }

        public Task<(User user, PasswordCredential passwordCredential)?> FindByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            if (User is null || PasswordCredential is null)
            {
                return Task.FromResult<
                    (User User, PasswordCredential PasswordCredential)?>(null);
            }

            if (User.Email != email)
            {
                return Task.FromResult<
                    (User User, PasswordCredential PasswordCredential)?>(null);
            }

            return Task.FromResult<
                (User User, PasswordCredential PasswordCredential)?>(
                    (User, PasswordCredential));
        }
    }
}