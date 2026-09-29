using AuthForge.Domain.Entities;
using AuthForge.Infrastructure.Persistence;
using AuthForge.IntegrationTests.Infrastructure;

namespace AuthForge.IntegrationTests.Users;

[Collection("Postgres")]
public class UserRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public UserRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task FindByIdAsync_WithPersistedUser_ReturnsUser()
    {
        await using var dbContext = _fixture.CreateDbContext();

        var user = new User("integration-find@example.com");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.FindByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithChangedUser_PersistsChanges()
    {
        await using var dbContext = _fixture.CreateDbContext();

        var user = new User("integration-update@example.com");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        user.PromoteToAdmin();

        var repository = new UserRepository(dbContext);

        await repository.UpdateAsync(user);

        await using var verificationContext = _fixture.CreateDbContext();

        var result = await verificationContext.Users
            .FindAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Role, result.Role);
    }
}