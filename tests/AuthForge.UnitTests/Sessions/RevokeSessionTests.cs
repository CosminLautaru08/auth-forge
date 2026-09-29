using AuthForge.Application.Sessions;
using AuthForge.Domain.Entities;

namespace AuthForge.UnitTests.Sessions;

public class RevokeSessionTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidSession_RevokesSession()
    {
        var userId = Guid.NewGuid();

        var session = new UserSession(
            userId,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1));

        var repository = new FakeSessionRepository
        {
            Session = session
        };

        var revokeSession = new RevokeSession(repository);

        await revokeSession.ExecuteAsync(
            session.Id,
            userId);

        Assert.NotNull(session.RevokedAt);
        Assert.Same(session, repository.UpdatedSession);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownSession_ThrowsInvalidOperationException()
    {
        var repository = new FakeSessionRepository
        {
            Session = null
        };

        var revokeSession = new RevokeSession(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            revokeSession.ExecuteAsync(
                Guid.NewGuid(),
                Guid.NewGuid()));

        Assert.Equal(
            "Invalid session.",
            exception.Message);

        Assert.Null(repository.UpdatedSession);
    }

    [Fact]
    public async Task ExecuteAsync_WithSessionBelongingToAnotherUser_ThrowsInvalidOperationException()
    {
        var sessionOwnerId = Guid.NewGuid();
        var authenticatedUserId = Guid.NewGuid();

        var session = new UserSession(
            sessionOwnerId,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1));

        var repository = new FakeSessionRepository
        {
            Session = session
        };

        var revokeSession = new RevokeSession(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            revokeSession.ExecuteAsync(
                session.Id,
                authenticatedUserId));

        Assert.Equal(
            "Invalid session.",
            exception.Message);

        Assert.Null(repository.UpdatedSession);
        Assert.Null(session.RevokedAt);
    }

    private class FakeSessionRepository : ISessionRepository
    {
        public UserSession? Session { get; set; }

        public UserSession? UpdatedSession { get; private set; }

        public Task AddAsync(
            UserSession session,
            CancellationToken cancellationToken = default)
        {
            _ = session;
            _ = cancellationToken;

            return Task.CompletedTask;
        }

        public Task<UserSession?> FindByIdAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            _ = sessionId;
            _ = cancellationToken;

            return Task.FromResult(Session);
        }

        public Task UpdateAsync(
            UserSession session,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            UpdatedSession = session;

            return Task.CompletedTask;
        }
    }
}