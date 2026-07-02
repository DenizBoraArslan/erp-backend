using HR.Application.Common;
using HR.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.LeaveRequests.ReviewLeaveRequest
{
    public class ReviewLeaveRequestCommandHandler : IRequestHandler<ReviewLeaveRequestCommand, Result<bool>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public ReviewLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Result<bool>> Handle(ReviewLeaveRequestCommand request, CancellationToken ct)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.LeaveRequestId, ct);
            if (leaveRequest is null)
                return Result<bool>.Failure("Leave request not found.");

            try
            {
                switch (request.Action.ToLower())
                {
                    case "approve": leaveRequest.Approve(request.Note); break;
                    case "reject": leaveRequest.Reject(request.Note); break;
                    case "cancel": leaveRequest.Cancel(); break;
                    default: return Result<bool>.Failure("Invalid action. Use: approve, reject, cancel.");
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result<bool>.Failure(ex.Message);
            }

            await _leaveRequestRepository.UpdateAsync(leaveRequest, ct);
            return Result<bool>.Success(true);
        }
    }
}
