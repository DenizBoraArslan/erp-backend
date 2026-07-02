using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Domain.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<LeaveRequest>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken ct = default);
        Task AddAsync(LeaveRequest leaveRequest, CancellationToken ct = default);
        Task UpdateAsync(LeaveRequest leaveRequest, CancellationToken ct = default);
    }
}
