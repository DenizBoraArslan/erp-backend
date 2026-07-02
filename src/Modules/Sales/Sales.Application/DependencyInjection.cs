using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Features.Orders.GetOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSalesApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(GetOrdersQueryHandler).Assembly));

            return services;
        }
    }
}
