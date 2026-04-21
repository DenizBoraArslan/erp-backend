using MediatR;
using Common.Results;
using Common.Interfaces;
using HR.Domain.Entities;

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
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(IRepository<Employee> employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                EmploymentType = (EmploymentType)request.EmploymentType,
                HireDate = request.HireDate,
                Salary = request.Salary,
                DepartmentId = request.DepartmentId,
                IsActive = true
            };

            await _employeeRepository.AddAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateEmployeeResponse
            {
                Id = employee.Id,
                Email = employee.Email
            };

            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<CreateEmployeeResponse>("Unexpected error occurred while creating employee.");
        }
    }
}

public class CreateEmployeeResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
