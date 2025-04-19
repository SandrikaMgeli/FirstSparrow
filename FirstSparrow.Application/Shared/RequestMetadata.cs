namespace FirstSparrow.Application.Shared;

public class RequestMetadata
{
    public readonly Guid TraceId = Guid.CreateVersion7();
}