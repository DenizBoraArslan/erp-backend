using HR.Application.Common;
using HR.Domain.Entities;
using HR.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.LeaveRequests.CreateLeaveRequest
{
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, Result<Guid>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateLeaveRequestCommandHandler(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeeRepository employeeRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<Guid>> Handle(CreateLeaveRequestCommand request, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, ct);
            if (employee is null)
                return Result<Guid>.Failure("Employee not found.");

            try
            {
                var leaveRequest = LeaveRequest.Create(
                    request.EmployeeId, request.StartDate, request.EndDate, request.Reason);

                await _leaveRequestRepository.AddAsync(leaveRequest, ct);
                return Result<Guid>.Success(leaveRequest.Id);
            }
            catch (InvalidOperationException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
