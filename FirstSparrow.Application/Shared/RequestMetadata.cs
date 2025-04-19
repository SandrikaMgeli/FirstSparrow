using FirstSparrow.Application.Domain.Enums;

namespace FirstSparrow.Application.Shared;

public class RequestMetadata
{
    public readonly Guid TraceId =  Guid.CreateVersion7();
}