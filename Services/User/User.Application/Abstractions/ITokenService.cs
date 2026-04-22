namespace User.Application.Abstractions;

public interface ITokenService
{
    AccessTokenDescriptor GenerateAccessToken(User.Domain.Entities.User user);
    RefreshTokenDescriptor GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}

public sealed record AccessTokenDescriptor(
    string Token,
    DateTime ExpiresAtUtc);

public sealed record RefreshTokenDescriptor(
    string Token,
    string TokenHash,
    DateTime ExpiresAtUtc);
