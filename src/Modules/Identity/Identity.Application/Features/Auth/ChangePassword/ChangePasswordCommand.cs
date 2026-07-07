namespace Identity.Application.Features.Auth.ChangePassword;

using Identity.Application.Common;
using MediatR;

// Self-service: any authenticated user changes their own password. UserId
// comes from the JWT, never from the request body — a user can never target
// someone else's account through this endpoint.
public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result<bool>>;
