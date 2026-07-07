using Finance.Application.Common;
using MediatR;
using System;

namespace Finance.Application.Features.Suppliers.CreateSupplier
{
    public record CreateSupplierCommand(
        string Name,
        string? ContactName = null,
        string? Email = null,
        string? Phone = null,
        string? Address = null,
        string? TaxNumber = null
    ) : IRequest<Result<Guid>>;
}
