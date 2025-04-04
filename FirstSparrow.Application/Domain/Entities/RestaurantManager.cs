using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Domain.Entities;

public class RestaurantManager : BaseEntity<int>
{
    public int RestaurantId { get; set; }

    public int ManagerId { get; set; }
}