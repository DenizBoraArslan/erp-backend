namespace Common.Extensions;

using Common.Messaging.Abstractions;
using Common.Messaging.InMemory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using Common.Behaviors;
using System.Text;

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

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key configuration is required.");
        if (key.Length < 32)
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters long.");

        var issuer = jwtSection["Issuer"] ?? "erp-backend";
        var audience = jwtSection["Audience"] ?? "erp-backend-clients";

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddOperationalFoundation(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.TryAddSingleton<InMemoryIntegrationEventChannel>();
        services.TryAddSingleton<IIntegrationEventPublisher, InMemoryIntegrationEventPublisher>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, InMemoryIntegrationEventProcessor>());

        return services;
    }
}
