using User.Domain.Entities;

namespace User.Application.Abstractions;

public interface IUserRepository
{
    Task<User.Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetActiveRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(User.Domain.Entities.User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User.Domain.Entities.User user, CancellationToken cancellationToken = default);
}
