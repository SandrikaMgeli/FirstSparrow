using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Domain.Entities;

public class Deposit : BaseEntity<int>
{
    public string Commitment { get; set; }

    public ulong  LeafIndex { get; set; }

    public DateTime DepositTimestamp { get; set; }
}