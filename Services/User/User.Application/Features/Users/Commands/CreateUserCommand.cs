global using UserDomainEntity = User.Domain.Entities.User;

using MediatR;
using Common.Results;
using Common.Interfaces;

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
    private readonly IRepository<UserDomainEntity> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IRepository<UserDomainEntity> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new UserDomainEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PhoneNumber = request.PhoneNumber,
                Role = (User.Domain.Entities.UserRole)request.Role,
                IsActive = true
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateUserResponse
            {
                Id = user.Id,
                Email = user.Email
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateUserResponse>(ex.Message);
        }
    }
}

public class CreateUserResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
