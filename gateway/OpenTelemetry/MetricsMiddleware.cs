using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Gateway.OpenTelemetry;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MetricsMiddleware> _logger;

    public MetricsMiddleware(RequestDelegate next, ILogger<MetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Obtén el nombre del endpoint desde el contexto o donde lo necesites
            var endpointName = context.Request.Path;

            // Registra métrica de tiempo de ejecución
            DiagnosticsConfig.TimeSpend.Record(stopwatch.Elapsed.TotalMilliseconds);

            // Registra métrica de cantidad de llamadas
            DiagnosticsConfig.TotalCalls.Add(1);

            _logger.LogInformation($"Endpoint: {endpointName}, Execution time: {stopwatch.Elapsed.TotalMilliseconds} ms");
        }
    }
}
