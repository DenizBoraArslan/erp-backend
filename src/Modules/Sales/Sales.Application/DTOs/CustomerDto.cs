using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.DTOs
{
    public record CustomerDto(
     Guid Id,
     string FirstName,
     string LastName,
     string Email,
     string? Phone,
     string? Address,
     bool IsActive
 );
}
