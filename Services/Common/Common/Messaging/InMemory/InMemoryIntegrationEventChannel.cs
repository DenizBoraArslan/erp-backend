using System.Threading.Channels;

namespace Common.Messaging.InMemory;

public class InMemoryIntegrationEventChannel
{
    public Channel<InMemoryIntegrationEventEnvelope> Channel { get; } =
        System.Threading.Channels.Channel.CreateUnbounded<InMemoryIntegrationEventEnvelope>();
}
