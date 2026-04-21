namespace Common.Extensions;

using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;
using AutoMapper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(assemblies);
        });

        return services;
    }
}
