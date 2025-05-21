using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Services.Models;
using FirstSparrow.Application.Shared;
using MediatR;

namespace FirstSparrow.Application.Features.Deposits.SyncDeposits;

public class SyncDepositsCommandHandler(
    IDepositRepository depositRepository,
    IMetadataRepository metadataRepository,
    IBlockChainService blockChainService) : IRequestHandler<SyncDepositsCommand, SyncDepositsResponse>
{
    private const int BatchSize = 500;
    public async Task<SyncDepositsResponse> Handle(SyncDepositsCommand request, CancellationToken cancellationToken)
    {
        ulong fromBlock = await GetLastCheckedBlock(cancellationToken);

        List<Deposit> deposits = await blockChainService.FetchDeposits(new FetchDepositsParams()
        {
            FromBlock = fromBlock, 
            BatchSize = BatchSize,
        }, cancellationToken);

        throw new NotImplementedException();
    }

    private async Task<ulong> GetLastCheckedBlock(CancellationToken cancellationToken)
    {
        Metadata metadata = (await metadataRepository.GetByKey(nameof(FirstSparrowConfigs.InitialBlockIndex), true, cancellationToken))!;

        return ulong.Parse(metadata.Value);
    }
}