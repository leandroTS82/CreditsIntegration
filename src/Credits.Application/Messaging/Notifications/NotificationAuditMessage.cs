namespace Credits.Application.Messaging.Notifications;

public sealed record NotificationAuditMessage(
    string EventType,
    string Key,
    DateTimeOffset OccurredAt);