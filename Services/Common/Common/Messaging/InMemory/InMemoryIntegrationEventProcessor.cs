using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.InMemory;

public class InMemoryIntegrationEventProcessor : BackgroundService
{
    private readonly InMemoryIntegrationEventChannel _channel;
    private readonly ILogger<InMemoryIntegrationEventProcessor> _logger;

    public InMemoryIntegrationEventProcessor(
        InMemoryIntegrationEventChannel channel,
        ILogger<InMemoryIntegrationEventProcessor> logger)
    {
        _channel = channel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var envelope in _channel.Channel.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation(
                "Processed integration event {EventType} at {OccurredOnUtc} with payload {Payload}",
                envelope.EventType,
                envelope.OccurredOnUtc,
                envelope.Payload);
        }
    }
}
