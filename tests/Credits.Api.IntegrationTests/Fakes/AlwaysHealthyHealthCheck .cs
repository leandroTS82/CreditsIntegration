using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Credits.Api.IntegrationTests.Fakes;

internal sealed class AlwaysHealthyHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
        => Task.FromResult(HealthCheckResult.Healthy("Fake - always healthy."));
}
