namespace MovieHub.Shared.Kernel.Infrastructure.Kafka;

/// <summary>
/// Interface for publishing events to Kafka
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
    Task PublishAsync<TEvent>(string topic, string key, TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
}
