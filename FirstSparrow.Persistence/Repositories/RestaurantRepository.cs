using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Extensions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Persistence.Constants;
using FirstSparrow.Persistence.Repositories.Base;

namespace FirstSparrow.Persistence.Repositories;

public class RestaurantRepository(DbManagementContext context) : IRestaurantRepository
{

    public async Task Insert(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        int restaurantId = await context.QuerySingleAsync<int>(RestaurantRepositoryQueries.InsertQuery, restaurant);
        restaurant.Id = restaurantId;
    }

    public async Task Update(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        await context.ExecuteAsync(RestaurantRepositoryQueries.UpdateQuery, restaurant);
    }

    public async Task<Restaurant> GetById(int id, bool ensureExists, CancellationToken cancellationToken = default)
    {
        Restaurant? restaurant = await context.QuerySingleOrDefaultAsync<Restaurant>(RestaurantRepositoryQueries.GetByIdQuery, new { Id = id });
        if (ensureExists)
        {
            restaurant.EnsureExists($"id = {id}");
        }

        return restaurant!;
    }

    public async Task<Restaurant> GetByName(string name, bool ensureExists, CancellationToken cancellationToken = default)
    {
        Restaurant? restaurant = await context.QuerySingleOrDefaultAsync<Restaurant>(RestaurantRepositoryQueries.GetByName, new { Name = name });
        if (ensureExists)
        {
            restaurant.EnsureExists($"name = {name}");
        }

        return restaurant!;
    }

    public async Task Delete(int id, CancellationToken cancellationToken = default)
    {
        await context.ExecuteAsync(RestaurantRepositoryQueries.DeleteQuery, new { Id = id });
    }
}