using Common.Interfaces;
using Common.Results;
using MediatR;
using User.Application.Abstractions;
using User.Domain.Entities;

namespace User.Application.Features.Auth.Commands;

public sealed class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponse>(
                "Login failed",
                new Error("INVALID_CREDENTIALS", "Email or password is invalid."));
        }

        foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
        {
            token.RevokedAtUtc = DateTime.UtcNow;
        }

        var refreshTokenDescriptor = _tokenService.GenerateRefreshToken();
        user.AddRefreshToken(new RefreshToken
        {
            TokenHash = refreshTokenDescriptor.TokenHash,
            ExpiresAtUtc = refreshTokenDescriptor.ExpiresAtUtc
        });

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
