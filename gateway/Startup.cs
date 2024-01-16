
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Utils;

namespace Gateway
{
    public class Startup
    {
        public const string ServiceName = "Bar API Gateway";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddConsul();
            services.AddHealthChecks();

            services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    .AddSource(ServiceName)
                    .ConfigureResource(resource => resource.AddService(ServiceName))
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://collector:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    }));
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

            app.UseRequestIdMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(ServiceName);
                });
            });

            app.UseOcelot();
        }
    }
}