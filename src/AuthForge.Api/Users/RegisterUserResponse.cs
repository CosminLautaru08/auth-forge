namespace AuthForge.Api.Users;

public record RegisterUserResponse(
    Guid Id,
    string Email
);