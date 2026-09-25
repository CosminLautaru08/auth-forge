using AuthForge.Application.Users;
using AuthForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthForge.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AuthForgeDbContext _dbContext;

    public UserRepository(AuthForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Id == userId,
                cancellationToken);
    }
}