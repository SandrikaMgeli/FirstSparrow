using System.Numerics;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Models;
using FirstSparrow.Application.Services.Models;

namespace FirstSparrow.Application.Services.Abstractions;

public interface IBlockChainService
{
    Task<List<Deposit>> FetchDeposits(FetchDepositsParams @params, CancellationToken cancellationToken = default);

    Task<BigInteger> HashConcat(BigInteger left, BigInteger right);
}