using Credits.Domain.Messaging.Messages;
using System.Text.Json;

namespace Credits.Worker.Deserializers;

public static class CreditMessageDeserializer
{
    public static IntegrateCreditMessage? Deserialize(string body)
        => JsonSerializer.Deserialize<IntegrateCreditMessage>(body);
}