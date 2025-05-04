using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommandHandler : IRequestHandler<RegisterRestaurantCommand, RegisterRestaurantResponse>
{
    public Task<RegisterRestaurantResponse> Handle(RegisterRestaurantCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RegisterRestaurantResponse());
    }
}