using HR.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.LeaveRequests.ReviewLeaveRequest
{
    public record ReviewLeaveRequestCommand(
      Guid LeaveRequestId,
      string Action,
      string? Note = null
  ) : IRequest<Result<bool>>;
}
