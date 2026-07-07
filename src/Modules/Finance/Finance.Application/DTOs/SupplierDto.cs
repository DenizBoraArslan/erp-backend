using System;

namespace Finance.Application.DTOs
{
    public record SupplierDto(
        Guid Id,
        string Name,
        string? ContactName,
        string? Email,
        string? Phone,
        string? Address,
        string? TaxNumber,
        bool IsActive,
        DateTime CreatedAt
    );
}
