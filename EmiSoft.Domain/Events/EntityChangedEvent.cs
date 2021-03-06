namespace EmiSoft.Domain.Events;

public class EntityChangedEvent<T> : DomainEventBase
{
    public T Data { get; }
    public EntityChangedEvent(T data)
    {
        ArgumentNullException.ThrowIfNull(nameof(data));
        Data = data;
    }
}
