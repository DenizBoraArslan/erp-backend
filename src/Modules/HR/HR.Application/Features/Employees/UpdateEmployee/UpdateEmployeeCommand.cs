using HR.Application.Common;
using MediatR;
using System;

namespace HR.Application.Features.Employees.UpdateEmployee
{
    public record UpdateEmployeeCommand(
        Guid EmployeeId,
        string FirstName,
        string LastName,
        string Position,
        decimal Salary,
        string? Phone = null
    ) : IRequest<Result<Guid>>;
}
