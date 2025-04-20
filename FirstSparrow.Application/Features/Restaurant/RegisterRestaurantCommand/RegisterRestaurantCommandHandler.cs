using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Enums;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Application.Services.Abstractions;
using MediatR;

namespace FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;

public class RegisterRestaurantCommandHandler(
    IDbManager dbManager,
    IRestaurantRepository restaurantRepository,
    ITimeProvider timeProvider,
    IOtpService otpService,
    IOtpRepository otpRepository) : IRequestHandler<RegisterRestaurantCommand, RegisterRestaurantResponse>
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
            UpdateTimestamp = timeProvider.GetUtcNow(),
            RestaurantFlags = RestaurantFlag.None
        };

        await using IDbManagementContext context = await dbManager.RunAsync(cancellationToken);

        await restaurantRepository.Insert(restaurant, cancellationToken);
        await SendOtp(restaurant.OwnerPhoneNumber);

        return new RegisterRestaurantResponse() { Id = restaurant.Id };
    }

    private async Task SendOtp(string phoneNumber)
    {
        int otp = otpService.Generate(4);
        DateTime currentTime = timeProvider.GetUtcNow();

        await otpRepository.Insert(new Otp()
        {
            Code = otp,
            Destination = phoneNumber,
            IsUsed = false,
            Usage = OtpUsage.OwnerPhoneVerification,
            IsSent = false,
            ExpiresAt = currentTime.AddMinutes(1),
            CreateTimestamp = currentTime,
            UpdateTimestamp = currentTime,
        });
    }
}