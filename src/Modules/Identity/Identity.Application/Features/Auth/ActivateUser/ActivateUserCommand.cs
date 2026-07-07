namespace Identity.Application.Features.Auth.ActivateUser;

using Identity.Application.Common;
using MediatR;

public record ActivateUserCommand(Guid UserId) : IRequest<Result<bool>>;
