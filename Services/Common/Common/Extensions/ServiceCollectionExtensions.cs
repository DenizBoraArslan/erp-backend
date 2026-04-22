namespace Common.Extensions;

using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using Common.Behaviors;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(assemblies);
        });

        return services;
    }
}
