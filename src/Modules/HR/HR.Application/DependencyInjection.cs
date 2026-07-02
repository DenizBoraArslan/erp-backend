using HR.Application.Features.Employees.GetEmployees;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHRApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(GetEmployeesQueryHandler).Assembly));

            return services;
        }
    }
}
