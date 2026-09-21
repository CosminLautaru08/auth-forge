using AuthForge.Application.Users;
using AuthForge.Domain.Entities;

namespace AuthForge.Infrastructure.Persistence;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly AuthForgeDbContext _dbContext;

    public RegistrationRepository(AuthForgeDbContext dbContext)
    {
        _dbContext = dbContext;
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