namespace FirstSparrow.Application.Shared;

public class RestaurantMetadata
{
    public readonly int RestaurantId;

    public readonly bool IsOnBoarded;

    public RestaurantMetadata(int restaurantId, bool isOnBoarded)
    {
        RestaurantId = restaurantId;
        IsOnBoarded = isOnBoarded;
    }
}