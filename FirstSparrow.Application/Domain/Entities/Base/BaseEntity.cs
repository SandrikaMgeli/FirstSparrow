namespace FirstSparrow.Application.Domain.Entities.Base;

public abstract class BaseEntity<TId> 
    where TId : struct, IComparable<TId>, IEquatable<TId>
{
    public TId Id { get; set; }

    public DateTime CreateTimestamp { get; set; }

    public DateTime? UpdateTimestamp { get; set; }

    public bool IsDeleted { get; set; }
}