using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Domain.Entities;

public class Manager : BaseEntity<int>
{
    public string Name { get; set; }

    public string LastName { get; set; }

    public string PhoneNumber { get; set; }
}