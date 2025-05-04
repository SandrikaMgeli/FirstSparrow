using FirstSparrow.Application.Domain.Exceptions;
using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommandHandler : IRequestHandler<RegisterRestaurantCommand, RegisterRestaurantResponse>
{
    public Task<RegisterRestaurantResponse> Handle(RegisterRestaurantCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RegisterRestaurantResponse());
    }

    private void EnsurePasswordIsValid(string password)
    {
        if(password.Length < 8) throw new AppException("Password must be at least 8 characters long");
    }
}