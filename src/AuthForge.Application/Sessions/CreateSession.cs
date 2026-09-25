using AuthForge.Domain.Entities;

namespace AuthForge.Application.Sessions;

public class CreateSession
{
    private readonly ISessionRepository _sessionRepository;

    public CreateSession(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<UserSession> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(1);

        var session = new UserSession(
            userId,
            now,
            expiresAt
        );

        await _sessionRepository.AddAsync(
            session,
            cancellationToken
        );

        return session;
    }
}