using Finance.Application.Common;
using MediatR;
using System;

namespace Finance.Application.Features.Suppliers.UpdateSupplier
{
    public record UpdateSupplierCommand(
        Guid SupplierId,
        string Name,
        string? ContactName = null,
        string? Phone = null,
        string? Address = null,
        string? TaxNumber = null
    ) : IRequest<Result<bool>>;
}
