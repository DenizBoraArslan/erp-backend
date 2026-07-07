namespace Identity.Application.Features.Auth.ActivateUser;

using Identity.Application.Common;
using Identity.Domain.Interfaces;
using MediatR;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(ActivateUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<bool>.Failure("Kullanıcı bulunamadı.");

        user.Activate();
        await _userRepository.UpdateAsync(user, ct);

        return Result<bool>.Success(true);
    }
}
