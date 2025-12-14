using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MovieHub.Shared.Kernel.Infrastructure.Kafka;

/// <summary>
/// Background service for consuming Kafka events
/// </summary>
public class KafkaBackgroundConsumer<TEvent> : BackgroundService where TEvent : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaBackgroundConsumer<TEvent>> _logger;
    private readonly ConsumerConfig _config;
    private readonly string _topic;
    private readonly int _maxRetries;

    public KafkaBackgroundConsumer(
        IServiceProvider serviceProvider,
        ILogger<KafkaBackgroundConsumer<TEvent>> logger,
        ConsumerConfig config,
        string topic,
        int maxRetries = 3)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config;
        _topic = topic;
        _maxRetries = maxRetries;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config).Build();
        consumer.Subscribe(_topic);

        _logger.LogInformation("Started consuming from topic: {Topic}", _topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    if (consumeResult?.Message == null)
                        continue;

                    await ProcessMessageAsync(consumeResult, stoppingToken);

                    consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from topic {Topic}", _topic);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopped for topic: {Topic}", _topic);
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessMessageAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
    {
        var attempt = 0;
        var processed = false;

        while (attempt < _maxRetries && !processed)
        {
            attempt++;

            try
            {
                var @event = JsonSerializer.Deserialize<TEvent>(consumeResult.Message.Value);

                if (@event == null)
                {
                    _logger.LogWarning("Failed to deserialize event from topic {Topic}", _topic);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IEventConsumer<TEvent>>();

                await handler.HandleAsync(@event, cancellationToken);

                _logger.LogInformation(
                    "Successfully processed event {EventType} from topic {Topic} at offset {Offset}",
                    typeof(TEvent).Name, _topic, consumeResult.Offset);

                processed = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing event {EventType} from topic {Topic} at offset {Offset} (attempt {Attempt}/{MaxRetries})",
                    typeof(TEvent).Name, _topic, consumeResult.Offset, attempt, _maxRetries);

                if (attempt >= _maxRetries)
                {
                    _logger.LogError("Max retries reached for event at offset {Offset}. Moving to DLQ.", consumeResult.Offset);
                    // TODO: Send to dead letter queue
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
        }
    }
}
