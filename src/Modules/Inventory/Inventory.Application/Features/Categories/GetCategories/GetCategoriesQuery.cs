using Inventory.Application.Common;
using Inventory.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Categories.GetCategories
{
    public record GetCategoriesQuery : IRequest<Result<IEnumerable<CategoryDto>>>;
}
