namespace Identity.Application.Features.Auth.ChangePassword;

using Identity.Application.Common;
using Identity.Domain.Interfaces;
using MediatR;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public ChangePasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            return Result<bool>.Failure("Yeni şifre en az 6 karakter olmalıdır.");

        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<bool>.Failure("Kullanıcı bulunamadı.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return Result<bool>.Failure("Mevcut şifre yanlış.");

        user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword(request.NewPassword));
        await _userRepository.UpdateAsync(user, ct);

        return Result<bool>.Success(true);
    }
}
