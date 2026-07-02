using HR.Application.Common;
using HR.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.Employees.CreateEmployee
{
    public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    Department Department,
    EmploymentType EmploymentType,
    string Position,
    decimal Salary,
    DateTime HireDate,
    string? Phone = null
) : IRequest<Result<Guid>>;
}
