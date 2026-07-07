using Finance.Application.Common;
using MediatR;
using System;

namespace Finance.Application.Features.Suppliers.ActivateSupplier
{
    public record ActivateSupplierCommand(Guid SupplierId) : IRequest<Result<bool>>;
}
