using AuthForge.Domain.Entities;
using AuthForge.Domain.Enums;

namespace AuthForge.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Suspend_ChangesStatusToSuspended()
    {
        var user = new User("user@example.com");

        user.Suspend();

        Assert.Equal(UserStatus.Suspended, user.Status);
    }

    [Fact]
    public void Activate_ChangesStatusToActive()
    {
        var user = new User("user@example.com");

        user.Suspend();
        user.Activate();

        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public void Constructor_DefaultsToUserRole()
    {
        var user = new User("user@example.com");

        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void PromoteToAdmin_ChangesRoleToAdmin()
    {
        var user = new User("user@example.com");

        user.PromoteToAdmin();

        Assert.Equal(UserRole.Admin, user.Role);
    }
}