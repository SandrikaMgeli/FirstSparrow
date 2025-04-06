using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Application.Services.Abstractions;
using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommandHandler(
    IDbManager dbManager,
    IRestaurantRepository restaurantRepository,
    ITimeProvider timeProvider) : IRequestHandler<RegisterRestaurantCommand, RegisterRestaurantResponse>
{
    public async Task<RegisterRestaurantResponse> Handle(RegisterRestaurantCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Restaurant restaurant = new()
        {
            Name = request.Name,
            OwnerPhoneNumber = request.OwnerPhoneNumber,
            OwnerName = request.OwnerName,
            OwnerLastName = request.OwnerLastName,
            OwnerPersonalNumber = request.OwnerPersonalNumber,
            CreateTimestamp = timeProvider.GetUtcNow(),
            UpdateTimestamp = timeProvider.GetUtcNow()
        };

        await dbManager.RunAsync(async ct =>
        {
            await restaurantRepository.Insert(restaurant, ct);
        }, cancellationToken);

        return new RegisterRestaurantResponse() { Id = restaurant.Id };
    }
}