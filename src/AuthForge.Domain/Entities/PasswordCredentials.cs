namespace AuthForge.Domain.Entities;

public class PasswordCredentials
{
    public Guid UserId { get; private set; }

    public string PasswordHash { get; private set; }

    public PasswordCredentials(Guid userId, string passwordHash)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        UserId = userId;
        PasswordHash = passwordHash;
    }
}
