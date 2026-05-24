using Credits.Domain.Messaging.Notifications;
using System.Text.Json;

namespace Credits.Worker.Deserializers;

public static class AuditMessageDeserializer
{
    public static NotificationAuditMessage? Deserialize(string body)
        => JsonSerializer.Deserialize<NotificationAuditMessage>(body);
}
