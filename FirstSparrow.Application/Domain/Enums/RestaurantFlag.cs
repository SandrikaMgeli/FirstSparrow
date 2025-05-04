namespace FirstSparrow.Application.Domain.Enums;

[Flags]
public enum RestaurantFlag
{
    None = 0,
    OnBoarded = 1_073_741_824, // 2 ** 30
}