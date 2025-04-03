namespace FirstSparrow.Application.Shared;

public class RequestMetadata
{
    public readonly Guid TraceId =  Guid.CreateVersion7();

    public bool IsCustomer { get; set; }

    /// <summary>
    /// From the business perspective 'Customer' is restaurant.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// From the business perspective 'User' is restaurant, which is not onboarded yet.
    /// </summary>
    public Guid? UserId { get; set; }
}