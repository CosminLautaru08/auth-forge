namespace AuthForge.Api.Users;

public record LoginUserResponse
(
    Guid SessionId,
    DateTime ExpiresAt
);