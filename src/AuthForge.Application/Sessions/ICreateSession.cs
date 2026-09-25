using AuthForge.Domain.Entities;

namespace AuthForge.Application.Sessions;

public interface ICreateSession
{
    Task<UserSession> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}