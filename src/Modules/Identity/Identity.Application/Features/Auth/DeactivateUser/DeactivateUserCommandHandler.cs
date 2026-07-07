namespace Identity.Application.Features.Auth.DeactivateUser;

using Identity.Application.Common;
using Identity.Domain.Interfaces;
using MediatR;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(DeactivateUserCommand request, CancellationToken ct)
    {
        if (request.UserId == request.RequestedByUserId)
            return Result<bool>.Failure("Kendi hesabınızı pasif yapamazsınız.");

        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<bool>.Failure("Kullanıcı bulunamadı.");

        user.Deactivate();
        await _userRepository.UpdateAsync(user, ct);

        return Result<bool>.Success(true);
    }
}
