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

    public DbSet<UserSession> UserSessions => Set<UserSession>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
        .HasKey(user => user.Id);

        modelBuilder.Entity<User>()
        .HasIndex(user => user.Email)
        .IsUnique();

        modelBuilder.Entity<PasswordCredential>()
        .HasKey(credential => credential.UserId);

        modelBuilder.Entity<PasswordCredential>()
        .HasOne<User>()
        .WithOne()
        .HasForeignKey<PasswordCredential>(
                credential => credential.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserSession>()
        .HasKey(session => session.Id);

    }

}




