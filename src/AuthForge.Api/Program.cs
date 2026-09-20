using AuthForge.Application.Security;
using AuthForge.Infrastructure.Security;
using AuthForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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

app.Run();