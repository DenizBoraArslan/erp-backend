namespace Common.Messaging.Contracts;

public interface IIntegrationEvent
{
    DateTime OccurredOnUtc { get; }
}
