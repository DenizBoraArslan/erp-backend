using Inventory.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Products.AddStock
{
    public record AddStockCommand(
     Guid ProductId,
     int Quantity,
     string? Note = null
 ) : IRequest<Result<bool>>;
}
