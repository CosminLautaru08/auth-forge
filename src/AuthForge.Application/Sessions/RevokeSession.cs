using AuthForge.Domain.Entities;

namespace AuthForge.Application.Sessions;

public class RevokeSession
{
    private readonly ISessionRepository _sessionRepository;

    public RevokeSession(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task ExecuteAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdAsync(
            sessionId,
            cancellationToken);

        if (session is null || session.UserId != userId)
        {
            throw new InvalidOperationException("Invalid session.");
        }

        session.Revoke();

        await _sessionRepository.UpdateAsync(
            session,
            cancellationToken);
    }
}