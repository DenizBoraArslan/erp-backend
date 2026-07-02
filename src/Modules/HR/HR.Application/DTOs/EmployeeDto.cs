using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application.DTOs
{
    public record EmployeeDto(
     Guid Id,
     string FirstName,
     string LastName,
     string Email,
     string? Phone,
     string Department,
     string EmploymentType,
     string Position,
     decimal Salary,
     DateTime HireDate,
     bool IsActive,
     DateTime CreatedAt
 );
}
