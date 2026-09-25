using AuthForge.Application.Sessions;
using AuthForge.Domain.Entities;


namespace AuthForge.UnitTests.Sessions;

public class CreateSessionTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidUserId_CreatesAndStoresSession()
    {
        var repository = new FakeSessionRepository();

        var createSession = new CreateSession(repository);

        var userId = Guid.NewGuid();

        var result = await createSession.ExecuteAsync(userId);


        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.True(result.ExpiresAt > result.CreatedAt);

        Assert.Same(result, repository.Session);
    }

    private class FakeSessionRepository : ISessionRepository
    {
        public UserSession? Session { get; private set; }

        public Task AddAsync(
            UserSession session,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            Session = session;

            return Task.CompletedTask;
        }
    }
}