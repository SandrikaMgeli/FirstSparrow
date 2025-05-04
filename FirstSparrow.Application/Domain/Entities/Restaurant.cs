using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Domain.Enums;

namespace FirstSparrow.Application.Domain.Entities;

public class Restaurant : BaseEntity<int>
{
    public string Name { get; set; }

    public string Password { get; set; }

    public RestaurantFlag RestaurantFlags { get; set; }
}