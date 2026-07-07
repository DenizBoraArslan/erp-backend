using Inventory.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Products.CreateProduct
{
    public record CreateProductCommand(
       string Name,
       decimal Price,
       Guid CategoryId,
       string? Description = null,
       int MinStockLevel = 0,
       decimal CostPrice = 0
       ) : IRequest<Result<Guid>>;
}

