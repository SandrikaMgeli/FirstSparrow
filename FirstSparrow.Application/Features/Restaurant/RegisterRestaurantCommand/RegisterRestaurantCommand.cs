using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommand : IRequest<RegisterRestaurantResponse>
{
    public string Name { get; set; }

    public string OwnerPhoneNumber { get; set; }

    public string OwnerName { get; set; }

    public string OwnerLastName { get; set; }

    public string OwnerPersonalNumber { get; set; }
}