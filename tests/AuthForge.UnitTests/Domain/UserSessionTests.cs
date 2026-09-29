using AuthForge.Domain.Entities;

namespace AuthForge.UnitTests.Domain;

public class UserSessionTests
{
    [Fact]
    public void NewSession_IsNotRevoked()
    {
        var createdAt = DateTime.UtcNow;

        var session = new UserSession(
            Guid.NewGuid(),
            createdAt,
            createdAt.AddHours(1));

        Assert.Null(session.RevokedAt);
    }

    [Fact]
    public void Revoke_SetsRevokedAt()
    {
        var createdAt = DateTime.UtcNow;

        var session = new UserSession(
            Guid.NewGuid(),
            createdAt,
            createdAt.AddHours(1));

        session.Revoke();

        Assert.NotNull(session.RevokedAt);
    }
}