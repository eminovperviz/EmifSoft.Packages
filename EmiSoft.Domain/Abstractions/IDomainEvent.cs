using MediatR;

namespace EmiSoft.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    public int Id { get; }
    public DateTime DateOccurred { get; }

}
