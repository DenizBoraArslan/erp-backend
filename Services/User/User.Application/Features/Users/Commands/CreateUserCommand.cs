using MediatR;
using Common.Results;

namespace User.Application.Features.Users.Commands;

public class CreateUserCommand : IRequest<Result<CreateUserResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Role { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement repository pattern and password hashing
        await Task.CompletedTask;
        return Result.Failure<CreateUserResponse>("Not implemented yet");
    }
}

public class CreateUserResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
