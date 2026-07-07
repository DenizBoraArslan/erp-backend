using erp.Shared.Contracts;
using Inventory.Application.Features.Products.GetProducts;
using Inventory.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInventoryApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(GetProductsQueryHandler).Assembly));

            // Cross-module read port: lets Sales validate stock availability
            // synchronously before creating/confirming an order.
            services.AddScoped<IProductStockProvider, ProductStockProvider>();

            return services;
        }
    }
}
