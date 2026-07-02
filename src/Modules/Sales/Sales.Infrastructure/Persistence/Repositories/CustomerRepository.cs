using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SalesDbContext _context;

        public CustomerRepository(SalesDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default)
            => await _context.Customers.Where(c => c.IsActive).ToListAsync(ct);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => await _context.Customers.AnyAsync(c => c.Email == email.ToLowerInvariant(), ct);

        public async Task AddAsync(Customer customer, CancellationToken ct = default)
        {
            await _context.Customers.AddAsync(customer, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(ct);
        }
    }
}
