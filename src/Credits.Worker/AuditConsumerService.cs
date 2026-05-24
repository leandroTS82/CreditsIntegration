using Azure.Messaging.ServiceBus;
using Credits.Application.Messaging.Settings;
using Credits.Worker.Processors;
using Credits.Worker.Processors.Abstractions;
using Microsoft.Extensions.Options;

namespace Credits.Worker;

public sealed class AuditConsumerService : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<AuditConsumerService> _logger;
    private readonly ServiceBusConsumerSettings _consumerSettings;
    private readonly ServiceBusReceiver _receiver;
    private readonly IAuditMessageProcessor _processor;

    public AuditConsumerService(
        ServiceBusClient client,
        IOptions<ServiceBusSettings> settings,
        ILogger<AuditConsumerService> logger,
        ILogger<AuditMessageProcessor> processorLogger)
    {
        _logger = logger;
        _consumerSettings = settings.Value.Consumer;

        _receiver = client.CreateReceiver(
            settings.Value.Topics.NotificationAudit,
            settings.Value.AuditSubscription,
            new ServiceBusReceiverOptions { ReceiveMode = ServiceBusReceiveMode.PeekLock });

        _processor = new AuditMessageProcessor(_receiver, processorLogger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "AuditConsumerService started. Polling every {Interval}ms.",
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
                _logger.LogError(ex, "Unexpected error while polling audit Service Bus.");
            }

            await Task.Delay(_consumerSettings.PollingIntervalMs, stoppingToken);
        }

        _logger.LogInformation("AuditConsumerService stopped.");
    }

    public async ValueTask DisposeAsync()
        => await _receiver.DisposeAsync();
}