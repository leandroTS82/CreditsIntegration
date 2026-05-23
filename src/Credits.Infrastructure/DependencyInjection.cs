using Azure.Messaging.ServiceBus;
using Credits.Application.Messaging.Abstractions;
using Credits.Application.Messaging.Settings;
using Credits.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Credits.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ServiceBusClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
            return new ServiceBusClient(settings.ConnectionString);
        });

        services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();

        return services;
    }
}
