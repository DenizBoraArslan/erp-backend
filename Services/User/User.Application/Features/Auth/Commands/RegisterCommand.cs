using Common.Interfaces;
using Common.Messaging.Abstractions;
using Common.Results;
using MediatR;
using User.Application.Abstractions;
using User.Application.Features.Auth.Events;
using User.Domain.Entities;

namespace User.Application.Features.Auth.Commands;

public sealed class RegisterCommand : IRequest<Result<AuthResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IIntegrationEventPublisher integrationEventPublisher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<AuthResponse>(
                "Registration failed",
                new Error("USER_ALREADY_EXISTS", "A user with this email already exists."));
        }

        var user = new User.Domain.Entities.User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.User,
            IsActive = true
        };

        var refreshTokenDescriptor = _tokenService.GenerateRefreshToken();
        user.AddRefreshToken(new RefreshToken
        {
            TokenHash = refreshTokenDescriptor.TokenHash,
            ExpiresAtUtc = refreshTokenDescriptor.ExpiresAtUtc
        });

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _integrationEventPublisher.PublishAsync(
            new UserRegisteredIntegrationEvent(user.Id, user.Email, user.Role.ToString()),
            cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return Result.Success(new AuthResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            accessToken.Token,
            refreshTokenDescriptor.Token,
            accessToken.ExpiresAtUtc,
            refreshTokenDescriptor.ExpiresAtUtc));
    }
}
