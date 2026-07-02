using Inventory.Application.Common;
using Inventory.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Products.GetProducts
{
    public record GetProductsQuery(Guid? CategoryId = null) : IRequest<Result<IEnumerable<ProductDto>>>;
}
