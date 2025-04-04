using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Domain.Entities;

public class Restaurant : BaseEntity<int>
{
    public string Name { get; set; }

    public string OwnerPhoneNumber { get; set; }

    public string OwnerName { get; set; }

    public string OwnerLastName { get; set; }

    public bool IsOnBoarded { get; set; }
}