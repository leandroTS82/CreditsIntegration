using Azure.Messaging.ServiceBus;
using Credits.Domain.Messaging.Settings;
using Credits.Worker.Processors;
using Credits.Worker.Processors.Abstractions;
using Microsoft.Extensions.Options;

namespace Credits.Worker;

public sealed class ServiceBusConsumerService : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<ServiceBusConsumerService> _logger;
    private readonly ServiceBusConsumerSettings _consumerSettings;
    private readonly ServiceBusReceiver _receiver;
    private readonly ICreditMessageProcessor _processor;

    public ServiceBusConsumerService(
    ServiceBusClient client,
    IOptions<ServiceBusSettings> settings,
    IServiceScopeFactory scopeFactory,
    ILogger<ServiceBusConsumerService> logger,
    ILogger<CreditMessageProcessor> processorLogger)
    {
        _logger = logger;
        _consumerSettings = settings.Value.Consumer;

        _receiver = client.CreateReceiver(
            settings.Value.Topics.IntegrateCreditConstituted,
            settings.Value.Subscription,
            new ServiceBusReceiverOptions { ReceiveMode = ServiceBusReceiveMode.PeekLock });

        _processor = new CreditMessageProcessor(scopeFactory, _receiver, processorLogger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "ServiceBusConsumerService started. Polling every {Interval}ms.",
            _consumerSettings.PollingIntervalMs);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _receiver.ReceiveMessageAsync(
                    maxWaitTime: TimeSpan.FromMilliseconds(_consumerSettings.MaxWaitTimeMs),
                    cancellationToken: stoppingToken);

                if (message is not null)
                    await _processor.ProcessAsync(message, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while polling Service Bus.");
            }

            await Task.Delay(_consumerSettings.PollingIntervalMs, stoppingToken);
        }

        _logger.LogInformation("ServiceBusConsumerService stopped.");
    }

    public async ValueTask DisposeAsync()
        => await _receiver.DisposeAsync();
}