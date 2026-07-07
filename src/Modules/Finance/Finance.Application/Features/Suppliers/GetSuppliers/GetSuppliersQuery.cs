using Finance.Application.Common;
using Finance.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Finance.Application.Features.Suppliers.GetSuppliers
{
    public record GetSuppliersQuery() : IRequest<Result<IEnumerable<SupplierDto>>>;
}
