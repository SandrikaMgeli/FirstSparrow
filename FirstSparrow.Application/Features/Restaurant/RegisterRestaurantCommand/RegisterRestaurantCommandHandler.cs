using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Repositories.Abstractions;
using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository) : IRequestHandler<RegisterRestaurantCommand, RegisterRestaurantResponse>
{
    public async Task<RegisterRestaurantResponse> Handle(RegisterRestaurantCommand request, CancellationToken cancellationToken)
    {
        try
        {

        }
        catch (AppException ex) when (ex.ExceptionCode == ExceptionCode.OBJECT_NOT_FOUND)
        {
            
        }
        
        return new RegisterRestaurantResponse();
    }

    private void EnsurePasswordIsValid(string password)
    {
        if(password.Length < 8) throw new AppException("Password must be at least 8 characters long");
    }
}