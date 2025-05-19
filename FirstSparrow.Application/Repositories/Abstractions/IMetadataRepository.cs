using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions.Base;

namespace FirstSparrow.Application.Repositories.Abstractions;

public interface IMetadataRepository : IBaseRepository<Metadata, int>
{
    Task<Metadata?> GetByKey(string key, bool ensureExists, CancellationToken cancellationToken = default);
}