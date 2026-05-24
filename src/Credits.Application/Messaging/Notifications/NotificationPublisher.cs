using Credits.Application.Messaging.Abstractions;
using Credits.Application.Messaging.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credits.Application.Messaging.Notifications;

public sealed class NotificationPublisher(
    IServiceBusPublisher publisher,
    IOptions<ServiceBusSettings> settings,
    ILogger<NotificationPublisher> logger) : INotificationPublisher
{
    private readonly ServiceBusSettings _settings = settings.Value;

    public async Task NotifyAsync(NotificationAuditMessage message, CancellationToken ct = default)
    {
        try
        {
            await publisher.PublishAsync(_settings.Topics.NotificationAudit, message, ct);

            logger.LogInformation(
                "Audit event published. EventType={EventType} Key={Key}",
                message.EventType,
                message.Key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to publish audit event. EventType={EventType} Key={Key}",
                message.EventType,
                message.Key);
        }
    }
}
