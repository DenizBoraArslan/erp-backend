using Finance.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFinanceModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddFinanceInfrastructure(configuration);
            return services;
        }
    }
}
