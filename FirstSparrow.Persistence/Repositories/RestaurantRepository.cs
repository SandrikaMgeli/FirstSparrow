using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Persistence.Constants;
using FirstSparrow.Persistence.Repositories.Base;

namespace FirstSparrow.Persistence.Repositories;

public class RestaurantRepository(DbManagementContext context) : IRestaurantRepository
{

    public async Task Insert(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        await context.ExecuteAsync(RestaurantRepositoryQueries.InsertQuery, restaurant);
    }

    public async Task Update(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        await context.ExecuteAsync(RestaurantRepositoryQueries.UpdateQuery, restaurant);
    }

    public async Task<Restaurant?> Get(int id, CancellationToken cancellationToken = default)
    {
        return await context.QuerySingleOrDefaultAsync<Restaurant>(RestaurantRepositoryQueries.GetByIdQuery, new { Id = id });
    }

    public Task Delete(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}