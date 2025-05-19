using Dapper;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Extensions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Persistence.Repositories.Base;

namespace FirstSparrow.Persistence.Repositories;

public class MetadataRepository(
    DbManagementContext context) : BaseRepository<Metadata, int>(context, MetadataRepositorySql.Insert, MetadataRepositorySql.Delete, MetadataRepositorySql.Update, MetadataRepositorySql.GetById), IMetadataRepository
{
    public async Task<Metadata?> GetByKey(string key, bool ensureExists, CancellationToken cancellationToken = default)
    {
        EnsureConnection();

        Metadata? metadata = await context.Connection!.QuerySingleOrDefaultAsync<Metadata>(MetadataRepositorySql.GetByKey, new { key }, context.Transaction);

        if(ensureExists)
        {
            metadata.EnsureExists($"key = {key}");
        }

        return metadata;
    }
}


public static class MetadataRepositorySql
{
    public const string GetByKey = $@"
                                    SELECT 
                                        id as {nameof(Metadata.Id)},
                                        key as {nameof(Metadata.Key)},
                                        value as {nameof(Metadata.Value)},
                                        create_timestamp as {nameof(Metadata.CreateTimestamp)},
                                        update_timestamp as {nameof(Metadata.UpdateTimestamp)},
                                        is_deleted as {nameof(Metadata.IsDeleted)}
                                    FROM metadata
                                    WHERE key = @key
                                      AND is_deleted = FALSE;";

    public const string Insert = @$"
                                    INSERT INTO metadata
                                        (
                                            key,
                                            value,
                                            create_timestamp,
                                            update_timestamp,
                                            is_deleted
                                        )
                                    VALUES 
                                        (
                                            @{nameof(Metadata.Key)},
                                            @{nameof(Metadata.Value)},
                                            @{nameof(Metadata.CreateTimestamp)},
                                            @{nameof(Metadata.UpdateTimestamp)},
                                            @{nameof(Metadata.IsDeleted)}
                                        )
                                    RETURNING id;";

    public const string GetById = null;

    public const string Update = @$"
                                    UPDATE metadata
                                        SET
                                            key = @{nameof(Metadata.Key)},
                                            value = @{nameof(Metadata.Value)},
                                            create_timestamp = @{nameof(Metadata.CreateTimestamp)},
                                            update_timestamp = @{nameof(Metadata.UpdateTimestamp)},
                                            is_deleted = @{nameof(Metadata.IsDeleted)}
                                    WHERE id = @{nameof(Metadata.Id)};";

    public const string Delete = null;
}