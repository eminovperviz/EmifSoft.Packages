using MediatR;

namespace EmiSoft.Domain.Events;
public abstract class BaseDomainEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
