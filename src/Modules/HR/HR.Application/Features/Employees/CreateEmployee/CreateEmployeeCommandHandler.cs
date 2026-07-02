using HR.Application.Common;
using HR.Domain.Entities;
using HR.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.Employees.CreateEmployee
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken ct)
        {
            var exists = await _employeeRepository.ExistsByEmailAsync(request.Email, ct);
            if (exists)
                return Result<Guid>.Failure("An employee with this email already exists.");

            var employee = Employee.Create(
                request.FirstName, request.LastName, request.Email,
                request.Department, request.EmploymentType,
                request.Position, request.Salary, request.HireDate, request.Phone);

            await _employeeRepository.AddAsync(employee, ct);

            return Result<Guid>.Success(employee.Id);
        }
    }
}
