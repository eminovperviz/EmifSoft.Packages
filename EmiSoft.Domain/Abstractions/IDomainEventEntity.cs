using EmiSoft.Domain.Events;

namespace EmiSoft.Domain.Abstractions;

public interface IDomainEventEntity
{
    List<BaseDomainEvent> Events { get; set; }
}
