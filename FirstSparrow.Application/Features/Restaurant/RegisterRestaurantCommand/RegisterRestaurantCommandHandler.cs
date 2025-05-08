using FirstSparrow.Application.Domain.Enums;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Application.Services.Abstractions;
using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    ICryptographyService cryptographyService,
    IDbManager dbManager) : IRequestHandler<RegisterRestaurantCommand, RegisterRestaurantResponse>
{
    public async Task<RegisterRestaurantResponse> Handle(RegisterRestaurantCommand request, CancellationToken cancellationToken)
    {
        EnsurePasswordIsValid(request.Password);
        try
        {
            await using IDbManagementContext context = await dbManager.RunAsync(cancellationToken);

            Domain.Entities.Restaurant restaurant = await restaurantRepository.GetByName(request.RestaurantName, true, cancellationToken);
            if (restaurant.Password is not null)
            {
                throw new AppException("Restaurant already Registered", ExceptionCode.GENERAL_ERROR);
            }

            restaurant.Password  = cryptographyService.Hash(request.Password);
            restaurant.RestaurantFlags |= RestaurantFlag.OnBoarded;

            await restaurantRepository.Update(restaurant, cancellationToken);
            return new RegisterRestaurantResponse() { Id = restaurant.Id };
        }
        catch (AppException ex) when (ex.ExceptionCode == ExceptionCode.OBJECT_NOT_FOUND)
        {
            throw new AppException(ex, "Restaurant not found", ExceptionCode.OBJECT_NOT_FOUND);
        }
    }

    private void EnsurePasswordIsValid(string password)
    {
        if (password.Length < 8)
        {
            throw new AppException("Password must be at least 8 characters long", ExceptionCode.PASSWORD_VALIDATION_ERROR);
        }
    }
}