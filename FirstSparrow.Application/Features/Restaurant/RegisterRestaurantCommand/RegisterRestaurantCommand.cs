using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommand : IRequest<RegisterRestaurantResponse>
{
    public string RestaurantName { get; set; }

    public string Password { get; set; }
}