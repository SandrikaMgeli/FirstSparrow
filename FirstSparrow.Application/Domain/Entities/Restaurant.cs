namespace FirstSparrow.Application.Domain.Entities;

public class Restaurant
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string OwnerPhoneNumber { get; set; }

    public string OwnerName { get; set; }

    public string OwnerLastName { get; set; }

    public bool IsOnBoarded { get; set; }
}