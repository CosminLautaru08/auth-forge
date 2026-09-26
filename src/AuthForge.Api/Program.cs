using AuthForge.Application.Security;
using AuthForge.Infrastructure.Security;
using AuthForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AuthForge.Application.Users;
using AuthForge.Api.Users;
using AuthForge.Application.Sessions;
using AuthForge.Api.Authentication;
using AuthForge.Domain.Entities;
using AuthForge.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<AuthenticationMiddleware>();

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

app.MapGet("/me", (HttpContext context) =>
{
    if (!context.Items.TryGetValue("User", out var value)
        || value is not User user)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new
    {
        user.Id,
        user.Email
    });
});

app.Run();