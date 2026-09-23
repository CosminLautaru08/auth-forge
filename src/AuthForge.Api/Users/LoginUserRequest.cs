namespace AuthForge.Api.Users;

public record LoginUserRequest
(
    string Email,
    string Password
);