using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Persistence.Repositories.Base;

namespace FirstSparrow.Persistence.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly DbManagementContext _context;

    public RestaurantRepository(DbManagementContext context)
    {
        _context = context;
    }

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