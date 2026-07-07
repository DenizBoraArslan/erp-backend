using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => await _context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

        public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
            => await _context.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync(ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
        }
    }
}
