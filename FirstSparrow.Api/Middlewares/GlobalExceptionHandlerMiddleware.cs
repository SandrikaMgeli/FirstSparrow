using System.Text.Json;
using FirstSparrow.Api.Controllers.Models;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Shared;

namespace FirstSparrow.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex) when (ex.ExceptionCode == ExceptionCode.UNAUTHORIZED)
        {
            await HandleUnauthorized(context);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private async Task HandleUnauthorized(HttpContext context)
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
}