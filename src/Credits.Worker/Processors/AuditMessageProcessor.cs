using Azure.Messaging.ServiceBus;
using Credits.Domain.Messaging.Notifications;
using Credits.Worker.Deserializers;
using Credits.Worker.Processors.Abstractions;

namespace Credits.Worker.Processors;

public sealed class AuditMessageProcessor(
    ServiceBusReceiver receiver,
    ILogger<AuditMessageProcessor> logger) : IAuditMessageProcessor
{
    public async Task ProcessAsync(ServiceBusReceivedMessage message, CancellationToken ct)
    {
        var payload = DeserializeOrDeadLetter(message, ct);
        if (payload is null) return;

        logger.LogInformation(
            "[AUDIT] EventType={EventType} | Key={Key} | OccurredAt={OccurredAt}",
            payload.EventType,
            payload.Key,
            payload.OccurredAt);

        await receiver.CompleteMessageAsync(message, ct);
    }

    private NotificationAuditMessage? DeserializeOrDeadLetter(
        ServiceBusReceivedMessage message, CancellationToken ct)
    {
        try
        {
            var payload = AuditMessageDeserializer.Deserialize(message.Body.ToString());
            if (payload is null)
            {
                logger.LogWarning("Audit message {MessageId} deserialized as null. Dead-lettering.", message.MessageId);
                _ = receiver.DeadLetterMessageAsync(message, "NullPayload", "Deserialization returned null.", ct);
                return null;
            }
            return payload;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize audit message {MessageId}. Dead-lettering.", message.MessageId);
            _ = receiver.DeadLetterMessageAsync(message, "DeserializationError", ex.Message, ct);
            return null;
        }
    }
}
