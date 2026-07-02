    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.DTOs
{
    public record OrderDto(
       Guid Id,
       string OrderNumber,
       Guid CustomerId,
       string CustomerName,
       string Status,
       decimal TotalAmount,
       string? Note,
       DateTime CreatedAt,
       List<OrderItemDto> OrderItems
   );
}
