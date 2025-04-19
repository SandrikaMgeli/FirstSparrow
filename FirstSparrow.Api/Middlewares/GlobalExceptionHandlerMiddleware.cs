using System.Text.Json;
using FirstSparrow.Api.Controllers.Models;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Shared;

namespace FirstSparrow.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    private const string UnknownError = "UNKNOWN_ERROR";
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex) when (ex.ExceptionCode == ExceptionCode.UNAUTHORIZED)
        {
            await HandleUnauthorized(context, ex);
        }
        catch (AppException ex)
        {
            await HandleAppException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownException(context, ex);
        }
    }

    private async Task HandleAppException(HttpContext context, AppException appException)
    {
        RequestMetadata requestMetadata = context.RequestServices.GetRequiredService<RequestMetadata>();
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        ApiProblemDetails apiProblemDetails = new ApiProblemDetails()
        {
            Message = appException.ExceptionCode.ToString(),
            TraceId = requestMetadata.TraceId,
        };

        var json = JsonSerializer.Serialize(apiProblemDetails);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleUnauthorized(HttpContext context, Exception ex)
    {
        RequestMetadata requestMetadata = context.RequestServices.GetRequiredService<RequestMetadata>();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        ApiProblemDetails apiProblemDetails = new ApiProblemDetails()
        {
            Message = nameof(ExceptionCode.UNAUTHORIZED),
            TraceId = requestMetadata.TraceId,
        };

        var json = JsonSerializer.Serialize(apiProblemDetails);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleUnknownException(HttpContext context, Exception ex)
    {
        RequestMetadata requestMetadata = context.RequestServices.GetRequiredService<RequestMetadata>();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        ApiProblemDetails apiProblemDetails = new ApiProblemDetails()
        {
            Message = UnknownError,
            TraceId = requestMetadata.TraceId,
        };

        var json = JsonSerializer.Serialize(apiProblemDetails);
        await context.Response.WriteAsync(json);
    }
}