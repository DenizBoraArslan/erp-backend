using HR.Application.Common;
using HR.Application.DTOs;
using HR.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.Employees.GetEmployees
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, Result<IEnumerable<EmployeeDto>>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<IEnumerable<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken ct)
        {
            var employees = await _employeeRepository.GetAllAsync(ct);

            var dtos = employees.Select(e => new EmployeeDto(
                e.Id, e.FirstName, e.LastName, e.Email, e.Phone,
                e.Department.ToString(), e.EmploymentType.ToString(),
                e.Position, e.Salary, e.HireDate, e.IsActive,
                e.CreatedAt
            ));

            return Result<IEnumerable<EmployeeDto>>.Success(dtos);
        }
    }
}
