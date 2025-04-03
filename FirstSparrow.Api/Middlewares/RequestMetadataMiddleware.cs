using FirstSparrow.Application.Shared;

namespace FirstSparrow.Api.Middlewares;

public class RequestMetadataMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        RequestMetadata requestMetadata = context.RequestServices.GetRequiredService<RequestMetadata>();

        SetTraceId(context, requestMetadata);

        await next(context);
    }

    private static void SetTraceId(HttpContext context, RequestMetadata requestMetadata)
    {
        context.Response.Headers.Append("TraceId", requestMetadata.TraceId.ToString());
    }
}