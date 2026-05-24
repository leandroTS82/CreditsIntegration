using Azure.Messaging.ServiceBus;

namespace Credits.Worker.Processors.Abstractions;

public interface ICreditMessageProcessor
{
    Task ProcessAsync(ServiceBusReceivedMessage message, CancellationToken ct);
}
