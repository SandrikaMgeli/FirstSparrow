using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Shared;

namespace FirstSparrow.Api.Controllers.Models;

public class ApiProblemDetails
{
    private const string UnknownError = "UNKNOWN_ERROR";

    public string Title { get; set; }

    public string? Message { get; set; }

    public Guid TraceId { get; set; }

    private ApiProblemDetails() { }

    public static ApiProblemDetails Create(AppException exception, RequestMetadata requestMetadata)
    {
        return new ApiProblemDetails()
        {
            Message = exception.AppMessage,
            TraceId = requestMetadata.TraceId,
            Title = exception.ExceptionCode.ToString(),
        };
    }

    public static ApiProblemDetails Create(RequestMetadata requestMetadata)
    {
        return new ApiProblemDetails()
        {
            Message = null,
            TraceId = requestMetadata.TraceId,
            Title = UnknownError,
        };
    }
}