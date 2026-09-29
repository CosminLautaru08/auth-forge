using AuthForge.Domain.Entities;
using AuthForge.Infrastructure.Persistence;
using AuthForge.IntegrationTests.Infrastructure;

namespace AuthForge.IntegrationTests.Sessions;

[Collection("Postgres")]
public class SessionRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public SessionRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_PersistsSession()
    {
        await using var dbContext = _fixture.CreateDbContext();

        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var expiresAt = createdAt.AddHours(1);

        var session = new UserSession(
            userId,
            createdAt,
            expiresAt);

        var repository = new SessionRepository(dbContext);

        await repository.AddAsync(session);

        await using var verificationContext =
            _fixture.CreateDbContext();

        var persistedSession =
            await verificationContext.UserSessions.FindAsync(session.Id);

        Assert.NotNull(persistedSession);
        Assert.Equal(session.Id, persistedSession.Id);
        Assert.Equal(userId, persistedSession.UserId);
        Assert.Equal(
            createdAt,
            persistedSession.CreatedAt,
            TimeSpan.FromMicroseconds(1));

        Assert.Equal(
            expiresAt,
            persistedSession.ExpiresAt,
            TimeSpan.FromMicroseconds(1));
    }

    [Fact]
    public async Task FindByIdAsync_WithPersistedSession_ReturnsSession()
    {
        await using var dbContext = _fixture.CreateDbContext();

        var createdAt = DateTime.UtcNow;
        var session = new UserSession(
            Guid.NewGuid(),
            createdAt,
            createdAt.AddHours(1));

        dbContext.UserSessions.Add(session);
        await dbContext.SaveChangesAsync();

        var repository = new SessionRepository(dbContext);

        var result = await repository.FindByIdAsync(session.Id);

        Assert.NotNull(result);
        Assert.Equal(session.Id, result.Id);
        Assert.Equal(session.UserId, result.UserId);
        Assert.Equal(session.CreatedAt, result.CreatedAt);
        Assert.Equal(session.ExpiresAt, result.ExpiresAt);
    }
}