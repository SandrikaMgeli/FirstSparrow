namespace FirstSparrow.Application.Domain.Enums;

[Flags]
public enum RestaurantFlag
{
    None = 0,
    OnBoarded = 1,
    OwnerNumberVerified = 2,
}