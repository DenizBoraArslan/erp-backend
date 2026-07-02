using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSalesModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSalesInfrastructure(configuration);
            return services;
        }
    }
}
