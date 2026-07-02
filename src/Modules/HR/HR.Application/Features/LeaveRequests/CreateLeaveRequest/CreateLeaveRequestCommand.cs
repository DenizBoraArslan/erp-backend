using HR.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.LeaveRequests.CreateLeaveRequest
{
    public record CreateLeaveRequestCommand(
     Guid EmployeeId,
     DateTime StartDate,
     DateTime EndDate,
     string Reason
 ) : IRequest<Result<Guid>>;
}
