using Finance.Application.Common;
using Finance.Application.DTOs;
using MediatR;
using System;

namespace Finance.Application.Features.Statements.GetSupplierStatement
{
    public record GetSupplierStatementQuery(Guid SupplierId) : IRequest<Result<StatementDto>>;
}
