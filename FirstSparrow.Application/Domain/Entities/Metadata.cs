using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Domain.Entities;

public class Metadata : BaseEntity<int>
{
    public string Key { get; set; }

    public string Value { get; set; }
}