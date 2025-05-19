using FirstSparrow.Application.Domain.Entities;

namespace FirstSparrow.Application.Services.Abstractions;

public interface IBlockChainService
{
    Task<List<Deposit>> FetchDeposits(int batchMaxSize, CancellationToken cancellationToken = default);
}