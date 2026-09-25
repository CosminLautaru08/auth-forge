using AuthForge.Application.Sessions;
using AuthForge.Domain.Entities;

namespace AuthForge.Infrastructure.Persistence;

public class SessionRepository : ISessionRepository
{
    private readonly AuthForgeDbContext _dbContext;

    public SessionRepository(AuthForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        UserSession session,
        CancellationToken cancellationToken = default
    )
    {
        await _dbContext.UserSessions.AddAsync(
            session,
            cancellationToken
        );

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}