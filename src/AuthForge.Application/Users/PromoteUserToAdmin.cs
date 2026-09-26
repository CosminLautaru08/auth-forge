using AuthForge.Domain.Entities;

namespace AuthForge.Application.Users;

public class PromoteUserToAdmin
{
    private readonly IUserRepository _userRepository;

    public PromoteUserToAdmin(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userRepository.FindByIdAsync(
            userId,
            cancellationToken
            );

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        user.PromoteToAdmin();

        await _userRepository.UpdateAsync(
            user,
            cancellationToken
        );

        return user;

    }
}