using System.Data;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Domain.Models;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Services.Models;
using FirstSparrow.Application.Shared;
using MediatR;

namespace FirstSparrow.Application.Features.Deposits.SyncDeposits;

public class SyncDepositsCommandHandler(
    IMetadataRepository metadataRepository,
    IBlockChainService blockChainService,
    IDepositService depositService,
    IDbManager dbManager) : IRequestHandler<SyncDepositsCommand>
{
    private const int BatchSize = 5000;

    public async Task Handle(SyncDepositsCommand request, CancellationToken cancellationToken)
    {
        // TODO: Take lock here
        await using IDbManagementContext context = await dbManager.RunWithTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        (ulong fromBlock, Metadata fromBlockEntity) = await GetLastCheckedBlock(cancellationToken);

        (List<Deposit> deposits, ulong lastBlockCheckedByFetchDeposit) = await FetchDeposits(fromBlock, cancellationToken);

        foreach (Deposit deposit in deposits)
        {
            await ProcessDeposit(deposit, cancellationToken);
        }

        // Update last checked block in metadata
        fromBlockEntity.Value = lastBlockCheckedByFetchDeposit.ToString();
        await metadataRepository.Update(fromBlockEntity, true, cancellationToken);

        await context.CommitAsync(cancellationToken);
    }

    private async Task ProcessDeposit(Deposit deposit, CancellationToken cancellationToken)
    {
        try
        {
            await depositService.ProcessDeposit(deposit, cancellationToken);
        }
        catch (AppException ex) when (ex.ExceptionCode == ExceptionCode.DEPOSIT_ALREADY_EXISTS)
        {
            // Ignore deposit already exists error.
        }
    }

    private async Task<(List<Deposit> deposits, ulong lastBlockChecked)> FetchDeposits(ulong fromBlock, CancellationToken cancellationToken)
    {
        (List<Deposit> deposits, ulong lastBlockChecked) = await blockChainService.FetchDeposits(new FetchDepositsParams()
        {
            FromBlock = fromBlock, 
            BatchSize = BatchSize,
        }, cancellationToken);

        // Ensure deposits Order by Index in ascending
        return (deposits.OrderBy(d => d.Index).ToList(), lastBlockChecked);
    }

    private async Task<(ulong lastCheckedBlock, Metadata metadataEntity)> GetLastCheckedBlock(CancellationToken cancellationToken)
    {
        Metadata metadata = (await metadataRepository.GetByKey(nameof(FirstSparrowConfigs.InitialBlockIndex), true, cancellationToken))!;

        return (ulong.Parse(metadata.Value), metadata);
    }
}