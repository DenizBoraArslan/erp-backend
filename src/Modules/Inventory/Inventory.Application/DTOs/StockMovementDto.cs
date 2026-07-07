using System;

namespace Inventory.Application.DTOs
{
    public record StockMovementDto(
        Guid Id,
        Guid ProductId,
        string ProductName,
        int Quantity,
        string Type,
        string? Note,
        DateTime CreatedAt
    );
}
