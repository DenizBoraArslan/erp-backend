using HR.Application.Common;
using HR.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Features.Employees.DeactivateEmployee
{
    public class DeactivateEmployeeCommandHandler : IRequestHandler<DeactivateEmployeeCommand, Result<bool>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public DeactivateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<bool>> Handle(DeactivateEmployeeCommand request, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, ct);
            if (employee is null)
                return Result<bool>.Failure("Personel bulunamadı.");

            employee.Deactivate();
            await _employeeRepository.UpdateAsync(employee, ct);

            return Result<bool>.Success(true);
        }
    }
}
