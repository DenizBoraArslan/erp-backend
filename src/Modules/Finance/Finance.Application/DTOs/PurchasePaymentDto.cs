using System;

namespace Finance.Application.DTOs
{
    public record PurchasePaymentDto(
        Guid Id,
        decimal Amount,
        string Method,
        string? Note,
        DateTime PaidAt
    );
}
