namespace FirstSparrow.Api.Controllers.Models;

public class ApiProblemDetails
{
    public required string Message { get; set; }

    public required Guid TraceId { get; set; }
}