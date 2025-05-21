using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace FirstSparrow.Infrastructure.Services.Ethereum.Models;

[Function("poseidon", "bytes32")]
public class PoseidonFunction : FunctionMessage
{
    [Parameter("bytes32[2]", "input", 1)]
    public byte[][] Input { get; set; }
}