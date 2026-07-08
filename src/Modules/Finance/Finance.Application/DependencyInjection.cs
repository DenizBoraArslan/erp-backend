using erp.Shared.Contracts;
using Finance.Application.Features.Invoices.GetInvoices;
using Finance.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFinanceApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(GetInvoicesQueryHandler).Assembly));

            services.AddScoped<IOrderInvoiceStatusProvider, OrderInvoiceStatusProvider>();

            return services;
        }
    }
}
