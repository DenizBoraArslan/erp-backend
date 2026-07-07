namespace Identity.Application.Features.Auth.ResetPassword;

using Identity.Application.Common;
using Identity.Domain.Interfaces;
using MediatR;

public class AdminResetPasswordCommandHandler
    : IRequestHandler<AdminResetPasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public AdminResetPasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(AdminResetPasswordCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            return Result<bool>.Failure("Şifre en az 6 karakter olmalıdır.");

        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<bool>.Failure("Kullanıcı bulunamadı.");

        user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword(request.NewPassword));
        await _userRepository.UpdateAsync(user, ct);

        return Result<bool>.Success(true);
    }
}
