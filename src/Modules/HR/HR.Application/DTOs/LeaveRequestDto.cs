using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.DTOs
{
    public record LeaveRequestDto(
     Guid Id,
     Guid EmployeeId,
     string EmployeeName,
     DateTime StartDate,
     DateTime EndDate,
     int TotalDays,
     string Reason,
     string Status,
     string? ReviewNote,
     DateTime CreatedAt
 );
}
