using Credits.Domain.Messaging.Notifications;

namespace Credits.Domain.Messaging.Abstractions;

public interface INotificationPublisher
{
    Task NotifyAsync(NotificationAuditMessage message, CancellationToken ct = default);
}
