using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Domain.Exceptions;

namespace FirstSparrow.Application.Domain.Extensions;

public static class EntityExtensions
{
    public static void EnsureExists<TEntity>(this TEntity? entity, string searchFilter) 
        where TEntity : class
    {
        if (entity == null)
        {
            throw new AppException($"{typeof(TEntity).Name} with search filter: {searchFilter} was not found",
                ExceptionCode.OBJECT_NOT_FOUND);
        }
    }
}