using HR.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Domain.Entities
{
    public class LeaveRequest
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }
        public Employee? Employee { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int TotalDays => (EndDate - StartDate).Days + 1;
        public string Reason { get; private set; } = string.Empty;
        public LeaveStatus Status { get; private set; }
        public string? ReviewNote { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private LeaveRequest() { }

        public static LeaveRequest Create(Guid employeeId, DateTime startDate, DateTime endDate, string reason)
        {
            if (endDate < startDate)
                throw new InvalidOperationException("End date cannot be before start date.");

            return new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                StartDate = startDate,
                EndDate = endDate,
                Reason = reason,
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Approve(string? note = null)
        {
            if (Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved.");
            Status = LeaveStatus.Approved;
            ReviewNote = note;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject(string? note = null)
        {
            if (Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected.");
            Status = LeaveStatus.Rejected;
            ReviewNote = note;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == LeaveStatus.Approved || Status == LeaveStatus.Rejected)
                throw new InvalidOperationException("Approved or rejected requests cannot be cancelled.");
            Status = LeaveStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
