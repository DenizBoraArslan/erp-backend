using HR.Application.Common;
using HR.Application.DTOs;
using HR.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.LeaveRequests.GetLeaveRequests
{
    public class GetLeaveRequestsQueryHandler : IRequestHandler<GetLeaveRequestsQuery, Result<IEnumerable<LeaveRequestDto>>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public GetLeaveRequestsQueryHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Result<IEnumerable<LeaveRequestDto>>> Handle(GetLeaveRequestsQuery request, CancellationToken ct)
        {
            var leaveRequests = request.EmployeeId.HasValue
                ? await _leaveRequestRepository.GetByEmployeeIdAsync(request.EmployeeId.Value, ct)
                : await _leaveRequestRepository.GetAllAsync(ct);

            var dtos = leaveRequests.Select(l => new LeaveRequestDto(
                l.Id,
                l.EmployeeId,
                $"{l.Employee?.FirstName} {l.Employee?.LastName}",
                l.StartDate,
                l.EndDate,
                l.TotalDays,
                l.Reason,
                l.Status.ToString(),
                l.ReviewNote,
                l.CreatedAt
            ));

            return Result<IEnumerable<LeaveRequestDto>>.Success(dtos);
        }
    }
}
