using FirstSparrow.Application.Shared;

namespace FirstSparrow.Api.Middlewares;

public class RequestMetadataMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        RequestMetadata requestMetadata = context.RequestServices.GetRequiredService<RequestMetadata>();
        SetupTraceIdResponseHeader(context, requestMetadata);

        await next(context);
    }

    private void SetupTraceIdResponseHeader(HttpContext context, RequestMetadata requestMetadata)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["trace_id"] = requestMetadata.TraceId.ToString();
            return Task.CompletedTask;
        });
    }
}