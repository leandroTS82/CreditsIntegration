using Credits.Application.Messaging.Notifications;

namespace Credits.Application.Messaging.Abstractions;

public interface INotificationPublisher
{
    Task NotifyAsync(NotificationAuditMessage message, CancellationToken ct = default);
}
