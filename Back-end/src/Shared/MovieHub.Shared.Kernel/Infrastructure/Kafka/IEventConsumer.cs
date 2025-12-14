namespace MovieHub.Shared.Kernel.Infrastructure.Kafka;

/// <summary>
/// Interface for consuming events from Kafka
/// </summary>
public interface IEventConsumer<TEvent> where TEvent : class
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
