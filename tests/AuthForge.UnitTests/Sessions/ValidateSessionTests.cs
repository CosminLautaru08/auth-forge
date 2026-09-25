using AuthForge.Application.Sessions;
using AuthForge.Application.Users;
using AuthForge.Domain.Entities;

namespace AuthForge.UnitTests.Sessions;

public class ValidateSessionTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidSession_ReturnsSession()
    {
        var user = new User("user@example.com");

        var session = new UserSession(
            user.Id,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1));

        var sessionRepository = new FakeSessionRepository
        {
            Session = session
        };

        var userRepository = new FakeUserRepository
        {
            User = user
        };

        var validateSession = new ValidateSession(sessionRepository, userRepository);

        var result = await validateSession.ExecuteAsync(session.Id);

        Assert.Same(session, result);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownSession_ThrowsInvalidOperationException()
    {
        var sessionRepository = new FakeSessionRepository
        {
            Session = null
        };

        var userRepository = new FakeUserRepository();

        var validateSession = new ValidateSession(
            sessionRepository,
            userRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validateSession.ExecuteAsync(Guid.NewGuid()));

        Assert.Equal(
            "Invalid session.",
            exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithExpiredSession_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();

        var session = new UserSession(
            userId,
            DateTime.UtcNow.AddHours(-2),
            DateTime.UtcNow.AddHours(-1));

        var sessionRepository = new FakeSessionRepository
        {
            Session = session
        };

        var userRepository = new FakeUserRepository();

        var validateSession = new ValidateSession(
            sessionRepository,
            userRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validateSession.ExecuteAsync(session.Id));

        Assert.Equal(
            "Session has expired.",
            exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithSessionForMissingUser_ThrowsInvalidOperationException()
    {
        var session = new UserSession(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1)
        );

        var sessionRepository = new FakeSessionRepository
        {
            Session = session
        };

        var userRepository = new FakeUserRepository
        {
            User = null
        };

        var validateSession = new ValidateSession(
            sessionRepository,
            userRepository
        );

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
        validateSession.ExecuteAsync(session.Id));

        Assert.Equal(
      "Invalid session.",
      exception.Message);
    }

    private class FakeSessionRepository : ISessionRepository
    {
        public UserSession? Session { get; set; }

        public Task AddAsync(
            UserSession session,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserSession?> FindByIdAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            _ = sessionId;
            _ = cancellationToken;

            return Task.FromResult(Session);
        }


    }

    private class FakeUserRepository : IUserRepository
    {
        public User? User { get; set; }

        public Task<User?> FindByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            _ = userId;
            _ = cancellationToken;

            return Task.FromResult(User);
        }
    }
}