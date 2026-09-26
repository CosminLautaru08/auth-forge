using AuthForge.Application.Sessions;
using AuthForge.Application.Users;

namespace AuthForge.Api.Authentication;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
       HttpContext context,
       ValidateSession validateSession,
       IUserRepository userRepository)
    {
        var authorization = context.Request.Headers.Authorization.ToString();

        if (authorization.StartsWith("Bearer "))
        {
            var sessionIdText = authorization["Bearer ".Length..];

            if (Guid.TryParse(sessionIdText, out var sessionId))
            {
                try
                {
                    var session = await validateSession.ExecuteAsync(
                        sessionId,
                        context.RequestAborted
                    );

                    var user = await userRepository.FindByIdAsync(
                        session.UserId,
                        context.RequestAborted
                    );

                    if (user is not null)
                    {
                        context.Items["User"] = user;
                    }
                }
                catch (InvalidOperationException)
                {

                    //Invalid session.


                }
            }
        }

        await _next(context);
    }
}