using HR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHRModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHRInfrastructure(configuration);
            return services;
        }
    }
}
