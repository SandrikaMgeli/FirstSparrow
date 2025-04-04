using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions;

namespace FirstSparrow.Persistence.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    public Task Insert(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Update(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Restaurant> Get(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}