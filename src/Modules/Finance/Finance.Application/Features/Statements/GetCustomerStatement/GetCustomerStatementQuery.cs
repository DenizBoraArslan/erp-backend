using Finance.Application.Common;
using Finance.Application.DTOs;
using MediatR;
using System;

namespace Finance.Application.Features.Statements.GetCustomerStatement
{
    public record GetCustomerStatementQuery(Guid CustomerId) : IRequest<Result<StatementDto>>;
}
