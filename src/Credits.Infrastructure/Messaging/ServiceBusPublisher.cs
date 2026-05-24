using Azure.Messaging.ServiceBus;
using Credits.Domain.Messaging.Abstractions;
using System.Text.Json;

namespace Credits.Infrastructure.Messaging;

public class ServiceBusPublisher : IServiceBusPublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    public ServiceBusPublisher(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task PublishAsync<T>(string topicName, T message, CancellationToken cancellationToken = default)
    {
        await using var sender = _client.CreateSender(topicName);
        var body = JsonSerializer.Serialize(message);
        var sbMessage = new ServiceBusMessage(body)
        {
            ContentType = "application/json"
        };
        await sender.SendMessageAsync(sbMessage, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
    }
}
