using HR.Domain.Entities;
using HR.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HRDbContext _context;

        public EmployeeRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

        public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct = default)
            => await _context.Employees.Where(e => e.IsActive).ToListAsync(ct);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => await _context.Employees.AnyAsync(e => e.Email == email.ToLowerInvariant(), ct);

        public async Task AddAsync(Employee employee, CancellationToken ct = default)
        {
            await _context.Employees.AddAsync(employee, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Employee employee, CancellationToken ct = default)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync(ct);
        }
    }
}
