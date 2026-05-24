namespace Credits.Domain.Messaging.Notifications;

public sealed record NotificationAuditMessage(
    string EventType,
    string Key,
    DateTimeOffset OccurredAt);