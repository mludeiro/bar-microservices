
using Gateway.OpenTelemetry;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Gateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddConsul();
            services.AddHealthChecks();

            services.AddOpenTelemetry()
                .ConfigureResource( resourcebuilder => 
                    resourcebuilder.AddService(DiagnosticsConfig.ServiceName))
                .WithTracing(tracerProviderBuilder =>
                    tracerProviderBuilder
                        .AddSource(DiagnosticsConfig.ServiceName)
                        .AddAspNetCoreInstrumentation()
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri("http://collector:4317");
                            options.Protocol = OtlpExportProtocol.Grpc;
                        }))
                .WithMetrics( metricsBuilder => {
                    metricsBuilder.AddAspNetCoreInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(DiagnosticsConfig.Meter.Name).AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://collector:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
                });

            services.AddLogging( l => {
                l.AddOpenTelemetry(o => {
                    o.SetResourceBuilder( ResourceBuilder.CreateDefault().AddService(DiagnosticsConfig.ServiceName))
                        .AddOtlpExporter();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // En producción, puedes configurar el manejo de errores de manera diferente
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            app.UseMiddleware<RequestIdMiddleware>();
            app.UseMiddleware<MetricsMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(DiagnosticsConfig.ServiceName);
                });
            });

//            app.UseOpenTelemetryPrometheusScrapingEndpoint();

            app.UseOcelot();
        }
    }
}