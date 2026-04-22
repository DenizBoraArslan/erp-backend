using Common.Interfaces;
using Common.Results;
using MediatR;
using User.Application.Abstractions;
using User.Domain.Entities;

namespace User.Application.Features.Auth.Commands;

public sealed class RefreshTokenCommand : IRequest<Result<AuthResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var refreshToken = await _userRepository.GetActiveRefreshTokenAsync(refreshTokenHash, cancellationToken);

        if (refreshToken is null)
        {
            return Result.Failure<AuthResponse>(
                "Refresh token failed",
                new Error("INVALID_REFRESH_TOKEN", "Refresh token is invalid or expired."));
        }

        refreshToken.RevokedAtUtc = DateTime.UtcNow;

        var newRefreshTokenDescriptor = _tokenService.GenerateRefreshToken();
        refreshToken.User.AddRefreshToken(new RefreshToken
        {
            TokenHash = newRefreshTokenDescriptor.TokenHash,
            ExpiresAtUtc = newRefreshTokenDescriptor.ExpiresAtUtc
        });

        await _userRepository.UpdateAsync(refreshToken.User, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(refreshToken.User);
        return Result.Success(new AuthResponse(
            refreshToken.User.Id,
            refreshToken.User.Email,
            refreshToken.User.Role.ToString(),
            accessToken.Token,
            newRefreshTokenDescriptor.Token,
            accessToken.ExpiresAtUtc,
            newRefreshTokenDescriptor.ExpiresAtUtc));
    }
}
