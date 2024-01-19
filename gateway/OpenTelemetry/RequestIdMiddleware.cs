using System.Diagnostics;

namespace Gateway.OpenTelemetry;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestId = Activity.Current?.TraceId.ToString();
        context.Response.Headers.Add("requestId", requestId ?? string.Empty);

        await _next(context);
    }
}
