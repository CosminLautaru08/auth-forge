using AuthForge.Application.Security;
using AuthForge.Application.Sessions;
using AuthForge.Application.Users;
using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.UnitTests.Users;

public class LoginUserTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsSession()
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

        var createSession = new FakeCreateSession();
        var passwordHasher = new FakePasswordHasher();

        var loginUser = new LoginUser(
            passwordHasher,
            repository,
            createSession);

        var result = await loginUser.ExecuteAsync(
            "user@example.com",
            "password123");

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Same(result, createSession.Session);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownEmail_ThrowsInvalidOperationException()
    {
        var passwordHasher = new FakePasswordHasher();
        var repository = new FakeLoginRepository();
        var createSession = new FakeCreateSession();

        var loginUser = new LoginUser(
            passwordHasher,
            repository,
            createSession);

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

        var passwordHasher = new FakePasswordHasher();
        var createSession = new FakeCreateSession();

        var loginUser = new LoginUser(
        passwordHasher,
        repository,
        createSession);

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

        var passwordHasher = new FakePasswordHasher();
        var createSession = new FakeCreateSession();

        var loginUser = new LoginUser(
            passwordHasher,
            repository,
            createSession);

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

    private class FakeCreateSession : ICreateSession
    {
        public UserSession? Session { get; private set; }

        public Task<UserSession> ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            var now = DateTime.UtcNow;

            var session = new UserSession(
                userId,
                now,
                now.AddHours(1));

            Session = session;

            return Task.FromResult(session);
        }
    }
}