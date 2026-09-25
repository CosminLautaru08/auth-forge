namespace AuthForge.Domain.Entities;

public class UserSession
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public UserSession(
        Guid userId,
        DateTime createdAt,
        DateTime expiresAt
    )
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User Id is Required.",
                nameof(userId)
            );
        }

        if (expiresAt <= createdAt)
        {
            throw new ArgumentException(
                "Session expiration must be after creation.",
                nameof(expiresAt)
            );
        }

        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }
}