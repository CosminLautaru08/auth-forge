using AuthForge.Application.Users;
using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.UnitTests.Users;

public class PromoteUserToAdminTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingUser_PromotesUserToAdmin()
    {
        var user = new User("user@example.com");

        var repository = new FakeUserRepository
        {
            User = user
        };

        var promoteUser = new PromoteUserToAdmin(repository);

        var result = await promoteUser.ExecuteAsync(user.Id);

        Assert.Equal(UserRole.Admin, result.Role);
        Assert.True(repository.UpdateCalled);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownUser_ThrowsInvalidOperationException()
    {
        var repository = new FakeUserRepository();

        var promoteUser = new PromoteUserToAdmin(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => promoteUser.ExecuteAsync(Guid.NewGuid()));

        Assert.Equal("User not found.", exception.Message);
    }

    private class FakeUserRepository : IUserRepository
    {
        public User? User { get; set; }

        public bool UpdateCalled { get; private set; }

        public Task<User?> FindByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (User is null || User.Id != userId)
            {
                return Task.FromResult<User?>(null);
            }

            return Task.FromResult<User?>(User);
        }

        public Task UpdateAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            UpdateCalled = true;
            User = user;

            return Task.CompletedTask;
        }
    }
}