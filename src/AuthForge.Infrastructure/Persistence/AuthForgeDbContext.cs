using System.Security.Cryptography.X509Certificates;
using AuthForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthForge.Infrastructure.Persistence;

public class AuthForgeDbContext : DbContext
{
    public AuthForgeDbContext(DbContextOptions<AuthForgeDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users => Set<User>();

    public DbSet<PasswordCredential> PasswordCredentials => Set<PasswordCredential>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
        .HasKey(user => user.Id);

        modelBuilder.Entity<PasswordCredential>()
        .HasKey(credential => credential.UserId);

    }
}




