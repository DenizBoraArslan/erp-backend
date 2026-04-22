namespace User.Application.Features.Auth;

public sealed record AuthResponse(
    int UserId,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc);
