using HR.Application.Common;
using HR.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.Features.Employees.GetEmployees
{
    public record GetEmployeesQuery : IRequest<Result<IEnumerable<EmployeeDto>>>;
}
