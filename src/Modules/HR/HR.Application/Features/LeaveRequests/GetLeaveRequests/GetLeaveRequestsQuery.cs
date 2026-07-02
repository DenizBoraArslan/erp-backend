using HR.Application.Common;
using HR.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.LeaveRequests.GetLeaveRequests
{
    public record GetLeaveRequestsQuery(Guid? EmployeeId = null) : IRequest<Result<IEnumerable<LeaveRequestDto>>>;
}
