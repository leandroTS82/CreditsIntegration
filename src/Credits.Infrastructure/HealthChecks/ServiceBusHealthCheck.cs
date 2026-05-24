using Azure.Messaging.ServiceBus;
using Credits.Domain.Messaging.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;


namespace Credits.Infrastructure.HealthChecks;

public sealed class ServiceBusHealthCheck(
    ServiceBusClient client,
    IOptions<ServiceBusSettings> settings) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var sender = client.CreateSender(
                settings.Value.Topics.IntegrateCreditConstituted);

            return HealthCheckResult.Healthy("The service bus is accessible.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("The service bus is inaccessible.", ex);
        }
    }
}
