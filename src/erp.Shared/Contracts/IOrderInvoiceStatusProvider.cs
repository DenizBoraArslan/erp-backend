namespace erp.Shared.Contracts;

/// <summary>
/// Cross-module read port: lets the Sales module ask whether the invoice
/// that Finance automatically creates for every new order (see
/// OrderCreatedConsumer) has been paid, without Sales taking a hard
/// dependency on Finance's Application/Domain layers.
///
/// This is deliberately NOT a MassTransit event: business-rule gates like
/// "can this order be delivered yet?" need a synchronous yes/no answer
/// within the same HTTP request, which an async pub/sub event can't give.
/// Since every module runs in the same erp.API process, a plain in-process
/// interface (implemented in Finance.Application, registered in the shared
/// DI container, and consumed from Sales.Application) is simpler and more
/// reliable than round-tripping through RabbitMQ request/response.
/// </summary>
public interface IOrderInvoiceStatusProvider
{
    Task<OrderInvoicePaymentStatus> GetPaymentStatusAsync(Guid orderId, CancellationToken ct = default);
}

public enum OrderInvoicePaymentStatus
{
    /// No invoice exists for this order at all.
    NoInvoice,
    Paid,
    /// Invoice exists but isn't fully paid yet (Draft/Issued/PartiallyPaid).
    NotPaid,
    Cancelled,
}
