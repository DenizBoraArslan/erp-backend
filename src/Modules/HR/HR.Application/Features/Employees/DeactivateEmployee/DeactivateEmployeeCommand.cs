using HR.Application.Common;
using MediatR;
using System;

namespace HR.Application.Features.Employees.DeactivateEmployee
{
    public record DeactivateEmployeeCommand(Guid EmployeeId) : IRequest<Result<bool>>;
}
