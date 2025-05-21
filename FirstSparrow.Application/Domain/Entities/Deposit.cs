using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Domain.Entities;

public class Deposit : BaseEntity<int>
{
    public string Commitment { get; set; }

    public uint Index { get; set; }

    public int Layer { get; set; }

    public DateTime DepositTimestamp { get; set; }
}