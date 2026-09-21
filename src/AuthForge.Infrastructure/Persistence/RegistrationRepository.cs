using AuthForge.Application.Users;
using AuthForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthForge.Infrastructure.Persistence;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly AuthForgeDbContext _dbContext;

    public RegistrationRepository(AuthForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
        .AnyAsync(
            user => user.Email == email,
            cancellationToken
        );
    }

    public async Task AddAsync(
        User user,
        PasswordCredential passwordCredential,
        CancellationToken cancellationToken = default
    )
    {
        _dbContext.Users.Add(user);
        _dbContext.PasswordCredentials.Add(passwordCredential);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}