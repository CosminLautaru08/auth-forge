using AuthForge.Application.Authorization;
using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.UnitTests.Authorization;

public class AuthorizationServiceTests
{
    [Fact]
    public void HasPermission_WithRegularUser_ReturnsFalse()
    {
        var user = new User("user@example.com");
        var authorizationService = new AuthorizationService();

        var result = authorizationService.HasPermission(
            user,
            Permission.AccessAdminArea);

        Assert.False(result);
    }

    [Fact]
    public void HasPermission_WithAdminUser_ReturnsTrue()
    {
        var user = new User("admin@example.com");
        user.PromoteToAdmin();

        var authorizationService = new AuthorizationService();

        var result = authorizationService.HasPermission(
            user,
            Permission.AccessAdminArea);

        Assert.True(result);
    }

    [Fact]
    public void HasPermission_WithAdminUser_CanManageUsers()
    {
        var user = new User("admin@example.com");
        user.PromoteToAdmin();

        var authorizationService = new AuthorizationService();

        var result = authorizationService.HasPermission(
            user,
            Permission.ManageUsers);

        Assert.True(result);
    }
}