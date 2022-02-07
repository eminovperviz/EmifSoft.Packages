using EmiSoft.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmiSoft.Domain;

public abstract class BaseEntity<T> : BaseEntity, IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; }
}


public class BaseEntity
{

}

