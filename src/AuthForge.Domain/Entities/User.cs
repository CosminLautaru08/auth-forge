using AuthForge.Domain.Enums;

namespace AuthForge.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public UserStatus Status { get; private set; }

    public UserRole Role { get; private set; }

    public User(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }
        Id = Guid.NewGuid();
        Email = email;
        Status = UserStatus.Active;
        Role = UserRole.User;
    }



    public void Suspend()
    {
        Status = UserStatus.Suspended;
    }
    public void Activate()
    {
        Status = UserStatus.Active;
    }

    public void PromoteToAdmin()
    {
        Role = UserRole.Admin;
    }
}