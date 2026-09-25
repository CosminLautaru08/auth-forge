using AuthForge.Application.Users;
using AuthForge.Domain.Entities;

namespace AuthForge.Application.Sessions;


public class ValidateSession
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;

    public ValidateSession(ISessionRepository sessionRepository,
    IUserRepository userRepository)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
    }

    public async Task<UserSession> ExecuteAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdAsync(
            sessionId,
            cancellationToken);

        if (session is null)
        {
            throw new InvalidOperationException("Invalid session.");
        }

        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Session has expired.");
        }

        var user = await _userRepository.FindByIdAsync(
           session.UserId,
           cancellationToken);


        if (user is null)
        {
            throw new InvalidOperationException("Invalid session.");
        }

        return session;
    }

}