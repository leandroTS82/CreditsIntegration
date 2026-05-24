namespace Credits.Domain.Messaging.Abstractions;

public interface IServiceBusPublisher
{
    Task PublishAsync<T>(string topicName, T message, CancellationToken cancellationToken = default);
}
