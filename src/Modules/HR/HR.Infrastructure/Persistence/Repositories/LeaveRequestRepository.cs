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
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly HRDbContext _context;

        public LeaveRequestRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id, ct);

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync(CancellationToken ct = default)
            => await _context.LeaveRequests
                .Include(l => l.Employee)
                .ToListAsync(ct);

        public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken ct = default)
            => await _context.LeaveRequests
                .Include(l => l.Employee)
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync(ct);

        public async Task AddAsync(LeaveRequest leaveRequest, CancellationToken ct = default)
        {
            await _context.LeaveRequests.AddAsync(leaveRequest, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(LeaveRequest leaveRequest, CancellationToken ct = default)
        {
            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync(ct);
        }
    }
}
