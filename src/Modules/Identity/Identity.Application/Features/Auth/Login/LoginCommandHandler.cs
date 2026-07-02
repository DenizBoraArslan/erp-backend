namespace Identity.Application.Features.Auth.Login;

using Identity.Application.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.Interfaces;
using MediatR;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
            return Result<AuthResponse>.Failure("Incorrect email or password.");

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
            return Result<AuthResponse>.Failure("Incorrect email or password.");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("Your account is not active.");

        var token = _tokenService.GenerateToken(user);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            user.Role.ToString(),
            token
        ));
    }
}