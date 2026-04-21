using MediatR;
using Common.Results;

namespace HR.Application.Features.Employees.Commands;

public class CreateEmployeeCommand : IRequest<Result<CreateEmployeeResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int EmploymentType { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<CreateEmployeeResponse>>
{
    public async Task<Result<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement repository pattern
        await Task.CompletedTask;
        return Result.Failure<CreateEmployeeResponse>("Not implemented yet");
    }
}

public class CreateEmployeeResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
