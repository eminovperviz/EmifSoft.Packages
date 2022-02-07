﻿using EmiSoft.Domain.Abstractions;

namespace EmiSoft.Domain;

public abstract class AuditableEntity<T> : BaseEntity<T>, IAuditableEntity
{
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}

