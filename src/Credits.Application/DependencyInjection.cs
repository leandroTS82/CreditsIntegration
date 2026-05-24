using Credits.Application.Abstractions;
using Credits.Domain.Messaging.Settings;
using Credits.Application.Services;
using Credits.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Credits.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ServiceBusSettings>()
        .Bind(configuration.GetSection(ServiceBusSettings.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

        services.AddScoped<IIntegrateCreditService, IntegrateCreditService>();
        services.AddScoped<IQueryCreditService, QueryCreditService>();
        services.AddScoped<IIngestCreditService, IngestCreditService>();

        services.AddValidatorsFromAssemblyContaining<IntegrateCreditCommandValidator>();
        return services;
    }
}
