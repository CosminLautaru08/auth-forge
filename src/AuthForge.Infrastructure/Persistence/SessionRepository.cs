using AuthForge.Application.Sessions;
using AuthForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<UserSession?> FindByIdAsync(
    Guid sessionId,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserSessions
            .FirstOrDefaultAsync(
                session => session.Id == sessionId,
                cancellationToken);
    }
}