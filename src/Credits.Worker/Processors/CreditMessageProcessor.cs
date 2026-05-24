using Azure.Messaging.ServiceBus;
using Credits.Application.Abstractions;
using Credits.Application.Commands;
using Credits.Domain.Messaging.Messages;
using Credits.Worker.Deserializers;
using Credits.Worker.Processors.Abstractions;

namespace Credits.Worker.Processors;

public sealed class CreditMessageProcessor(
    IServiceScopeFactory scopeFactory,
    ServiceBusReceiver receiver,
    ILogger<CreditMessageProcessor> logger) : ICreditMessageProcessor
{
    public async Task ProcessAsync(ServiceBusReceivedMessage message, CancellationToken ct)
    {
        var payload = DeserializeOrDeadLetter(message, ct);
        if (payload is null) return;

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<IIngestCreditService>();

            var command = new IngestCreditCommand(
                payload.CreditNumber,
                payload.NfseNumber,
                payload.ConstitutionDate,
                payload.IssqnAmount,
                payload.CreditType,
                payload.IsSimpleNational,
                payload.TaxRate,
                payload.BilledAmount,
                payload.DeductionAmount,
                payload.CalculationBase);

            await service.IngestAsync(command, ct);
            await receiver.CompleteMessageAsync(message, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process message {MessageId}. Abandoning.", message.MessageId);
            await receiver.AbandonMessageAsync(message, cancellationToken: ct);
        }
    }

    private IntegrateCreditMessage? DeserializeOrDeadLetter(
        ServiceBusReceivedMessage message, CancellationToken ct)
    {
        try
        {
            var payload = CreditMessageDeserializer.Deserialize(message.Body.ToString());
            if (payload is null)
            {
                logger.LogWarning("Message {MessageId} deserialized as null. Dead-lettering.", message.MessageId);
                _ = receiver.DeadLetterMessageAsync(message, "NullPayload", "Deserialization returned null.", ct);
                return null;
            }
            return payload;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize message {MessageId}. Dead-lettering.", message.MessageId);
            _ = receiver.DeadLetterMessageAsync(message, "DeserializationError", ex.Message, ct);
            return null;
        }
    }
}