using Inventory.Application.Common;
using Inventory.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace Inventory.Application.Features.StockMovements.GetStockMovements
{
    public record GetStockMovementsQuery(Guid? ProductId = null) : IRequest<Result<IEnumerable<StockMovementDto>>>;
}
