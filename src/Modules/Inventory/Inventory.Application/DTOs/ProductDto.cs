using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DTOs
{
    public record ProductDto(
     Guid Id,
     string Name,
     string? Description,
     string SKU,
     decimal Price,
     decimal CostPrice,
     int StockQuantity,
     int MinStockLevel,
     bool IsLowStock,
     Guid CategoryId,
     string CategoryName,
     bool IsActive,
     DateTime CreatedAt
 );
}
