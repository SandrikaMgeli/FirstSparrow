using FirstSparrow.Application.Domain.Enums;

namespace FirstSparrow.Application.Shared;

public class RequestMetadata
{
    public readonly Guid TraceId =  Guid.CreateVersion7();

    /// <summary>
    /// If RestaurantMetadata is null, then request is coming from restaurant's visitor
    /// </summary>
    public RestaurantMetadata? RestaurantMetadata { get; set; }

    public Role CallerRole { get; set; }
}