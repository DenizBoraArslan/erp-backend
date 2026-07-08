namespace Identity.Application.Features.Auth.ChangePassword;

using Identity.Application.Common;
using MediatR;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result<bool>>;
