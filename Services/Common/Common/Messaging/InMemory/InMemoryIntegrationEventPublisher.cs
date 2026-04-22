using Common.Messaging.Abstractions;
using Common.Messaging.Contracts;
using System.Text.Json;

namespace Common.Messaging.InMemory;

public class InMemoryIntegrationEventPublisher : IIntegrationEventPublisher
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly InMemoryIntegrationEventChannel _channel;

    public InMemoryIntegrationEventPublisher(InMemoryIntegrationEventChannel channel)
    {
        _channel = channel;
    }

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var envelope = new InMemoryIntegrationEventEnvelope(
            integrationEvent.GetType().Name,
            JsonSerializer.Serialize(integrationEvent, JsonSerializerOptions),
            integrationEvent.OccurredOnUtc);

        await _channel.Channel.Writer.WriteAsync(envelope, cancellationToken);
    }
}
