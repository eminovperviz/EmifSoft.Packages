using EmiSoft.Domain.Abstractions;

namespace EmiSoft.Domain;

public abstract class AuditableEntityBase<T> : EntityBase<T>, IAuditableEntity
{
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}

