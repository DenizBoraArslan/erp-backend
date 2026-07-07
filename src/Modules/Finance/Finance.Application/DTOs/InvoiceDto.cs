using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.DTOs
{
    public record InvoiceDto(
      Guid Id,
      string InvoiceNumber,
      Guid CustomerId,
      string CustomerName,
      Guid? OrderId,
      string Status,
      decimal SubTotal,
      decimal TaxTotal,
      decimal TotalAmount,
      decimal PaidAmount,
      decimal RemainingAmount,
      DateTime IssuedAt,
      DateTime? DueDate,  
      List<InvoiceItemDto> Items,
      List<PaymentDto> Payments
    );
}
