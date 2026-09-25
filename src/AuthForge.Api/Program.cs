using AuthForge.Application.Security;
using AuthForge.Infrastructure.Security;
using AuthForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AuthForge.Application.Users;
using AuthForge.Api.Users;
using AuthForge.Application.Sessions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<RegisterUser>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<LoginUser>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ICreateSession, CreateSession>();
builder.Services.AddScoped<ValidateSession>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<AuthForgeDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AuthForge")
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapPost("/users", async (
    RegisterUserRequest request,
    RegisterUser registerUser,
    CancellationToken cancellationToken) =>
{
    var result = await registerUser.ExecuteAsync(
        request.Email,
        request.Password,
        cancellationToken);

    return Results.Ok(
        new RegisterUserResponse(
            result.User.Id,
            result.User.Email));
});

app.MapPost("/login", async (
    LoginUserRequest request,
    LoginUser loginUser,
    CancellationToken cancellationToken
) =>
{
    try
    {
        var session = await loginUser.ExecuteAsync(
            request.Email,
            request.Password,
            cancellationToken
        );

        return Results.Ok(
            new LoginUserResponse(
               session.Id,
                session.ExpiresAt
            ));
    }
    catch (InvalidOperationException)
    {
        return Results.Unauthorized();
    }
});

app.MapGet("/me", async (
    HttpRequest httpRequest,
    ValidateSession validateSession,
    IUserRepository userRepository,
    CancellationToken cancellationToken
) =>
{
    var authorization = httpRequest.Headers.Authorization.ToString();

    if (!authorization.StartsWith("Bearer "))
    {
        return Results.Unauthorized();
    }

    var sessionIdText = authorization["Bearer ".Length..];

    if (!Guid.TryParse(sessionIdText, out var sessionId))
    {
        return Results.Unauthorized();
    }

    try
    {
        var session = await validateSession.ExecuteAsync(
            sessionId,
            cancellationToken);

        var user = await userRepository.FindByIdAsync(
            session.UserId,
            cancellationToken);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new
        {
            user.Id,
            user.Email
        });
    }
    catch (InvalidOperationException)
    {
        return Results.Unauthorized();
    }
});

app.Run();