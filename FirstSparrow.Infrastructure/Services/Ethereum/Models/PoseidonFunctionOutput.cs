using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FirstSparrow.Infrastructure.Services.Ethereum.Models;

[FunctionOutput]
public class PoseidonFunctionOutput : IFunctionOutputDTO
{
    [Parameter("bytes32", "result", 1)]
    public virtual BigInteger Result { get; set; }
}