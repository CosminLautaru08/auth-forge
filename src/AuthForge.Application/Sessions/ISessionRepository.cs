using AuthForge.Domain.Entities;

namespace AuthForge.Application.Sessions;

public interface ISessionRepository
{
    Task AddAsync(
        UserSession session,
        CancellationToken cancellationToken = default
    );
}