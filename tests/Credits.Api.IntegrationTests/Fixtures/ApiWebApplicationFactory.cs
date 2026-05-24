using Credits.Application.Messaging.Abstractions;
using Credits.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.PostgreSql;

namespace Credits.Api.IntegrationTests.Fixtures;
public sealed class ApiWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("credits_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public Mock<IServiceBusPublisher> ServiceBusPublisherMock { get; } = new();

    public IServiceBusPublisher ServiceBusPublisher => ServiceBusPublisherMock.Object;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ServiceBus:ConnectionString"] = "fake-connection-string",
                ["ServiceBus:Subscription"] = "credits-subscription",
                ["ServiceBus:Topics:IntegrateCreditConstituted"] = "integrar-credito-constituido-entry",
                ["ServiceBus:Consumer:PollingIntervalMs"] = "500",
                ["ServiceBus:Consumer:MaxWaitTimeMs"] = "100",
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));

            var publisherDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IServiceBusPublisher));
            if (publisherDescriptor is not null)
                services.Remove(publisherDescriptor);

            services.AddSingleton(ServiceBusPublisher);
        });

        builder.UseEnvironment("Test");
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}