namespace Identity.Application.Features.Auth.DeactivateUser;

using Identity.Application.Common;
using MediatR;

public record DeactivateUserCommand(Guid UserId, Guid RequestedByUserId) : IRequest<Result<bool>>;
