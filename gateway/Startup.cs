
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Utils;

namespace Gateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddConsul();
            services.AddHealthChecks();

            services.AddOpenTelemetry().WithTracing( tracing => {
                tracing.AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = (httpContext) => !httpContext.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase);
                });
                tracing.AddJaegerExporter(jaegerOptions =>
                {
                    jaegerOptions.AgentHost = "jaeger";
                    jaegerOptions.AgentPort = 6831;
                });
                tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("gateway"));
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

            app.UseRequestIdMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Bar API Gateway");
                });
            });

            app.UseOcelot();
        }
    }
}