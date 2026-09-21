using AuthForge.Application.Security;
using AuthForge.Application.Users;
using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.UnitTests.Users;

public class RegisterUserTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_CreatesActiveUserAndPasswordCredential()
    {
        var repository = new FakeRegistrationRepository();

        var registerUser = new RegisterUser(
            new FakePasswordHasher(),
            repository);

        var result = await registerUser.ExecuteAsync(
            "user@example.com",
            "password123");

        Assert.Equal("user@example.com", result.User.Email);
        Assert.Equal(UserStatus.Active, result.User.Status);
        Assert.NotEqual(Guid.Empty, result.User.Id);

        Assert.Equal(
            result.User.Id,
            result.PasswordCredential.UserId);

        Assert.Equal(
            "hashed:password123",
            result.PasswordCredential.PasswordHash);

        Assert.Same(result.User, repository.User);
        Assert.Same(
            result.PasswordCredential,
            repository.PasswordCredential);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyEmail_ThrowsArgumentException()
    {
        var registerUser = new RegisterUser(
            new FakePasswordHasher(),
            new FakeRegistrationRepository());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            registerUser.ExecuteAsync("", "password123"));
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        var repository = new FakeRegistrationRepository
        {
            ExistingEmail = "user@example.com"
        };

        var registerUser = new RegisterUser(
            new FakePasswordHasher(),
            repository
        );

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
        registerUser.ExecuteAsync(
            "user@example.com",
            "password123"
        ));
    }


    private class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }

    private class FakeRegistrationRepository : IRegistrationRepository
    {
        public string? ExistingEmail { get; set; }

        public User? User { get; private set; }

        public PasswordCredential? PasswordCredential { get; private set; }

        public Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            return Task.FromResult(
                ExistingEmail == email);
        }

        public Task AddAsync(
            User user,
            PasswordCredential passwordCredential,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            User = user;
            PasswordCredential = passwordCredential;

            return Task.CompletedTask;
        }
    }
}