using System;

namespace Identity.Application.DTOs
{
    public record UserSummaryDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAt
    );
}
