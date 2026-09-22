using AuthForge.Application.Users;
using AuthForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthForge.Infrastructure.Persistence;

public class LoginRepository : ILoginRepository
{
    private readonly AuthForgeDbContext _dbContext;

    public LoginRepository(AuthForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(User user, PasswordCredential passwordCredential)?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Email == email,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        var passwordCredential = await _dbContext.PasswordCredentials
            .FirstOrDefaultAsync(
                credential => credential.UserId == user.Id,
                cancellationToken);

        if (passwordCredential is null)
        {
            return null;
        }

        return (user, passwordCredential);
    }
}