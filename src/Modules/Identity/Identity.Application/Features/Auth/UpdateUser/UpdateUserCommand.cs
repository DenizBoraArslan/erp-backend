namespace Identity.Application.Features.Auth.UpdateUser;

using Identity.Application.Common;
using MediatR;

public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role
) : IRequest<Result<Guid>>;
