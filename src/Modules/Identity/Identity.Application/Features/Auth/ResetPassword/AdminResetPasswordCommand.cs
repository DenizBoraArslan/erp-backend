namespace Identity.Application.Features.Auth.ResetPassword;

using Identity.Application.Common;
using MediatR;


public record AdminResetPasswordCommand(Guid UserId, string NewPassword) : IRequest<Result<bool>>;
