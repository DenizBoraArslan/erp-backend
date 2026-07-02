using Inventory.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Categories.ActivateCategory
{
    public record ActivateCategoryCommand(Guid CategoryId) : IRequest<Result<bool>>;
}
