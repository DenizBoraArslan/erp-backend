namespace Identity.Application.Features.Auth.GetUsers;

using Identity.Application.Common;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserSummaryDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserSummaryDto>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await _userRepository.GetAllAsync(ct);

        var dtos = users
            .Select(u => new UserSummaryDto(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Role.ToString(),
                u.IsActive,
                u.CreatedAt))
            .ToList();

        return Result<List<UserSummaryDto>>.Success(dtos);
    }
}
