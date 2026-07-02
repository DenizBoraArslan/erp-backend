using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs
{
    public record AuthResponse(
     Guid UserId,
     string Email,
     string FullName,
     string Role,
     string Token
 );
}
