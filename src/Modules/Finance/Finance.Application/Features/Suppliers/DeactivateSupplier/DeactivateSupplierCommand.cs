using Finance.Application.Common;
using MediatR;
using System;

namespace Finance.Application.Features.Suppliers.DeactivateSupplier
{
    public record DeactivateSupplierCommand(Guid SupplierId) : IRequest<Result<bool>>;
}
