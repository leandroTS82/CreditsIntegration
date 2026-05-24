using Azure.Messaging.ServiceBus;

namespace Credits.Worker.Processors.Abstractions;

public interface IAuditMessageProcessor
{
    Task ProcessAsync(ServiceBusReceivedMessage message, CancellationToken ct);
}