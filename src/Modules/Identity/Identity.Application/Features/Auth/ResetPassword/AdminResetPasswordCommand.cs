namespace Identity.Application.Features.Auth.ResetPassword;

using Identity.Application.Common;
using MediatR;

// Admin-only: force-sets another user's password (e.g. they're locked out).
public record AdminResetPasswordCommand(Guid UserId, string NewPassword) : IRequest<Result<bool>>;
