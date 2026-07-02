using MediatR;
using Sales.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Orders.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(
     Guid OrderId,
     string Action
 ) : IRequest<Result<bool>>;
}
