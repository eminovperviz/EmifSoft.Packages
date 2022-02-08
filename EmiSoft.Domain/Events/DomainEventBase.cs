using EmiSoft.Domain.Abstractions;

namespace EmiSoft.Domain.Events;
public abstract class DomainEventBase : IDomainEvent
{
    public int Id { get; protected set; }
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
