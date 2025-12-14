using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MovieHub.Shared.Kernel.Infrastructure.Kafka;

/// <summary>
/// Kafka event publisher implementation
/// </summary>
public class KafkaEventPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;

    public KafkaEventPublisher(ProducerConfig config, ILogger<KafkaEventPublisher> logger)
    {
        _producer = new ProducerBuilder<string, string>(config).Build();
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var key = Guid.NewGuid().ToString();
        await PublishAsync(topic, key, @event, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(string topic, string key, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        try
        {
            var message = JsonSerializer.Serialize(@event);

            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = message,
                Headers = new Headers
                {
                    { "event-type", System.Text.Encoding.UTF8.GetBytes(typeof(TEvent).Name) },
                    { "timestamp", System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("o")) }
                }
            }, cancellationToken);

            _logger.LogInformation(
                "Published event {EventType} to topic {Topic} with key {Key} at offset {Offset}",
                typeof(TEvent).Name, topic, key, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType} to topic {Topic}", typeof(TEvent).Name, topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
