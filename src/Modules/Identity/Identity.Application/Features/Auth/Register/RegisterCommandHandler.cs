namespace Identity.Application.Features.Auth.Register;

using Identity.Application.Common;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using MediatR;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var exists = await _userRepository.ExistsByEmailAsync(request.Email, ct);
        if (exists)
            return Result<AuthResponse>.Failure("This email address is already in use.");

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            return Result<AuthResponse>.Failure($"Geçersiz rol: {request.Role}");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = User.Create(request.FirstName, request.LastName, request.Email, passwordHash, role);

        await _userRepository.AddAsync(user, ct);

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