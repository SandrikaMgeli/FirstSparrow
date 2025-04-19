using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Domain.Exceptions;

namespace FirstSparrow.Application.Domain.Extensions;

public static class EntityExtensions
{
    public static void EnsureExists<TEntity, TId>(this TEntity? entity, TId searchedId) 
        where TId : struct
        where TEntity : BaseEntity<TId>
    {
        if (entity == null)
        {
            throw new AppException($"{typeof(TEntity).Name} with id: {searchedId} was not found",
                ExceptionCode.OBJECT_NOT_FOUND);
        }
    }
}