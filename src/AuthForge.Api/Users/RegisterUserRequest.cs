namespace AuthForge.Api.Users;

public record RegisterUserRequest
(
    string Email,
    string Password
);