using Inventory.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Categories.DeactivateCategory
{
    public record DeactivateCategoryCommand(Guid CategoryId) : IRequest<Result<bool>>;
}
