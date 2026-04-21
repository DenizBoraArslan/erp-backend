using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models;

public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public abstract record DomainEvent
{ 
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
