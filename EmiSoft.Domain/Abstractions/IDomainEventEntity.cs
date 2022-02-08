namespace EmiSoft.Domain.Abstractions;

public interface IDomainEventEntity
{
    List<IDomainEvent> Events { get; set; }
}
