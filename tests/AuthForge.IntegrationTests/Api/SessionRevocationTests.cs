using System.Net;
using System.Net.Http.Headers;
using AuthForge.Domain.Entities;
using AuthForge.IntegrationTests.Infrastructure;

namespace AuthForge.IntegrationTests.Api;

[Collection("Postgres")]
public class SessionRevocationTests
{
    private readonly PostgresFixture _fixture;

    public SessionRevocationTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RevokeSession_WithValidSession_ReturnsOk()
    {
        await using var dbContext = _fixture.CreateDbContext();

        var user = new User("api-revoke@example.com");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var session = new UserSession(
            user.Id,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1));

        dbContext.UserSessions.Add(session);
        await dbContext.SaveChangesAsync();

        await using var factory =
            new AuthForgeWebApplicationFactory(
                _fixture.ConnectionString);

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                session.Id.ToString());

        var response = await client.PostAsync(
            "/sessions/revoke",
            null);

        response.EnsureSuccessStatusCode();

        await using var verificationContext =
            _fixture.CreateDbContext();

        var persistedSession =
            await verificationContext.UserSessions
                .FindAsync(session.Id);

        Assert.NotNull(persistedSession);
        Assert.NotNull(persistedSession.RevokedAt);
    }

    [Fact]
    public async Task RevokedSession_CannotAccessAuthenticatedEndpoint()
    {
        await using var dbContext = _fixture.CreateDbContext();

        var user = new User("api-revoked@example.com");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var session = new UserSession(
            user.Id,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1));

        dbContext.UserSessions.Add(session);
        await dbContext.SaveChangesAsync();

        await using var factory =
            new AuthForgeWebApplicationFactory(
                _fixture.ConnectionString);

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                session.Id.ToString());

        var revokeResponse = await client.PostAsync(
            "/sessions/revoke",
            null);

        revokeResponse.EnsureSuccessStatusCode();

        var meResponse = await client.GetAsync("/me");

        Assert.Equal(
            System.Net.HttpStatusCode.Unauthorized,
            meResponse.StatusCode);
    }

    [Fact]
    public async Task GetMe_WithUnknownSession_ReturnsUnauthorized()
    {
        await using var factory =
            new AuthForgeWebApplicationFactory(
                _fixture.ConnectionString);

        using var client = factory.CreateClient();

        var unknownSessionId = Guid.NewGuid();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                unknownSessionId.ToString());

        var response = await client.GetAsync("/me");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}