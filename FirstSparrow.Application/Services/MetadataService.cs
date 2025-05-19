using System.Numerics;
using System.Reflection;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Shared;
using Microsoft.Extensions.Options;

namespace FirstSparrow.Application.Services;

public class MetadataService(
    IOptions<FirstSparrowConfigs> firstSparrowConfigs,
    IMetadataRepository metadataRepository,
    IDbManager dbManager,
    TimeProvider timeProvider) : IMetadataService
{
    public async Task InitializeNecessaryMetadata(CancellationToken cancellationToken = default)
    {
        await using IDbManagementContext context = await dbManager.RunAsync(cancellationToken);

        await InitializeBlockIndexKey(cancellationToken);
    }

    private async Task InitializeBlockIndexKey(CancellationToken cancellationToken)
    {
        string blockIndexKey = nameof(FirstSparrowConfigs.InitialBlockIndex);
        var metadata = await GetOrCreateMetadata(cancellationToken, blockIndexKey);

        if (uint.Parse(metadata.Value) < firstSparrowConfigs.Value.InitialBlockIndex)
        {
            metadata.Value = firstSparrowConfigs.Value.InitialBlockIndex.ToString();
            metadata.UpdateTimestamp = timeProvider.GetUtcNow().DateTime;
    
            await metadataRepository.Update(metadata, true, cancellationToken);
        }
    }

    private async Task<Metadata> GetOrCreateMetadata(CancellationToken cancellationToken, string blockIndexKey)
    {
        Metadata? metadata = await metadataRepository.GetByKey(blockIndexKey, false, cancellationToken);

        if (metadata == null)
        {
            metadata = new Metadata()
            {
                Key = blockIndexKey,
                Value = firstSparrowConfigs.Value.InitialBlockIndex.ToString(),
                CreateTimestamp = timeProvider.GetUtcNow().DateTime,
                UpdateTimestamp = null,
                IsDeleted = false,
            };
            await metadataRepository.Insert(metadata, cancellationToken);
        }

        return metadata;
    }
}