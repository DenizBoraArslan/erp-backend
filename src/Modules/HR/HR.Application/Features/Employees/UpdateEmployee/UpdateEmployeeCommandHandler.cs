using HR.Application.Common;
using HR.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<Guid>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public UpdateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<Guid>> Handle(UpdateEmployeeCommand request, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, ct);
            if (employee is null)
                return Result<Guid>.Failure("Personel bulunamadı.");

            employee.Update(request.FirstName, request.LastName, request.Position, request.Salary, request.Phone);
            await _employeeRepository.UpdateAsync(employee, ct);

            return Result<Guid>.Success(employee.Id);
        }
    }
}
