namespace FirstSparrow.Application.Shared;

public class RequestMetadata
{
    public readonly Guid TraceId =  Guid.CreateVersion7();

    public bool IsRestaurantRequest => CallerRole != BusinessRole.RestaurantVisitor;

    /// <summary>
    /// If RestaurantMetadata is null, then request is coming from restaurant's visitor visitor
    /// </summary>
    public RestaurantMetadata? RestaurantMetadata { get; set; }

    public BusinessRole CallerRole { get; set; }
}