using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Credits.Domain.Messaging.Abstractions;
using Credits.Domain.Messaging.Notifications;
using Credits.Domain.Messaging.Settings;
using Credits.Domain.Repositories;
using Credits.Infrastructure.HealthChecks;
using Credits.Infrastructure.Messaging;
using Credits.Infrastructure.Persistence;
using Credits.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Credits.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ServiceBusClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
            return new ServiceBusClient(settings.ConnectionString);
        });

        services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();

        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICreditRepository, CreditRepository>();
        services.AddScoped<INotificationPublisher, NotificationPublisher>();

        services.AddScoped<ServiceBusHealthCheck>();

        services.AddHealthChecks()
        .AddNpgSql(
            configuration.GetConnectionString("DefaultConnection")!,
            name: "postgres",
            tags: ["ready"])
        .AddCheck<ServiceBusHealthCheck>("servicebus", tags: ["ready"]);

        return services;
    }
}
