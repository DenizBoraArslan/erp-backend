using Inventory.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInventoryModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddInventoryInfrastructure(configuration);
            return services;
        }
    }
}
