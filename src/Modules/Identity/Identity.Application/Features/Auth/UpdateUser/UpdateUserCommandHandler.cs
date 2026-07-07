namespace Identity.Application.Features.Auth.UpdateUser;

using Identity.Application.Common;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using MediatR;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<Guid>.Failure("Kullanıcı bulunamadı.");

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            return Result<Guid>.Failure($"Geçersiz rol: {request.Role}");

        user.Update(request.FirstName, request.LastName);
        user.UpdateRole(role);

        await _userRepository.UpdateAsync(user, ct);

        return Result<Guid>.Success(user.Id);
    }
}
