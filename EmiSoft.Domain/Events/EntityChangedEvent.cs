namespace EmiSoft.Domain.Events;

public class EntityChangedEvent<T> : BaseDomainEvent
{
    public T Data { get; }
    public EntityChangedEvent(T data)
    {
        ArgumentNullException.ThrowIfNull(nameof(data));
        Data = data;
    }
}
