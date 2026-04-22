using Common.Messaging.Contracts;

namespace User.Application.Features.Auth.Events;

public sealed record UserRegisteredIntegrationEvent(
    int UserId,
    string Email,
    string Role) : IIntegrationEvent
{
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
