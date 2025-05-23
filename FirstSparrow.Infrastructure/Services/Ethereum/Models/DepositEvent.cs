using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FirstSparrow.Infrastructure.Services.Ethereum.Models;

[Event(DepositEventConstants.EventName)]
public class DepositEvent : IEventDTO
{
    [Parameter("bytes32", "commitment", 1, true)]
    public virtual BigInteger? Commitment { get; set; }

    [Parameter("uint32", "leafIndex", 2, false)]
    public virtual uint LeafIndex { get; set; }

    [Parameter("uint256", "timestamp", 3, false)]
    public virtual long Timestamp { get; set; } 
}

public static class DepositEventConstants
{
    public const string EventName = "Deposit";
}