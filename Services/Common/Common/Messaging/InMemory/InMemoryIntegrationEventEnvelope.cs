namespace Common.Messaging.InMemory;

public sealed record InMemoryIntegrationEventEnvelope(
    string EventType,
    string Payload,
    DateTime OccurredOnUtc);
