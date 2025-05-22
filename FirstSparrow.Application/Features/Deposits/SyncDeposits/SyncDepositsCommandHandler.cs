using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Models;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Services.Models;
using FirstSparrow.Application.Shared;
using MediatR;

namespace FirstSparrow.Application.Features.Deposits.SyncDeposits;

public class SyncDepositsCommandHandler(
    IMetadataRepository metadataRepository,
    IBlockChainService blockChainService,
    IDepositService depositService) : IRequestHandler<SyncDepositsCommand>
{
    private const int BatchSize = 500;

    public async Task Handle(SyncDepositsCommand request, CancellationToken cancellationToken)
    {
        ulong fromBlock = await GetLastCheckedBlock(cancellationToken);

        List<Deposit> deposits = await FetchDeposits(fromBlock, cancellationToken);

        foreach (Deposit deposit in deposits)
        {
            await depositService.ProcessDeposit(deposit, cancellationToken);
        }
    }

    private async Task<List<Deposit>> FetchDeposits(ulong fromBlock, CancellationToken cancellationToken)
    {
        List<Deposit> deposits = await blockChainService.FetchDeposits(new FetchDepositsParams()
        {
            FromBlock = fromBlock, 
            BatchSize = BatchSize,
        }, cancellationToken);

        // Ensures that Deposits are ordered
        return deposits.OrderBy(d => d.Index).ToList();
    }

    private async Task<ulong> GetLastCheckedBlock(CancellationToken cancellationToken)
    {
        Metadata metadata = (await metadataRepository.GetByKey(nameof(FirstSparrowConfigs.InitialBlockIndex), true, cancellationToken))!;

        return ulong.Parse(metadata.Value);
    }
}