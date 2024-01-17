using System.Diagnostics;
using System.Reflection;
using BrandService.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BrandService
{
    public class Startup
    {
        public const string ServiceName = "Brand Service";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            services.AddDbContext<BarContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("BarConnection"))
            );
            
            services.AddHealthChecks().AddDbContextCheck<BarContext>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    .AddSource(ServiceName)
                    .ConfigureResource(resource => resource.AddService(ServiceName))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = (httpContext) => !httpContext.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase);
                    })
                    .AddConsoleExporter()
                    .AddEntityFrameworkCoreInstrumentation( t => t.SetDbStatementForText = true)
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://collector:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    }));

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestIdMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers(); 
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            using var scope = app.ApplicationServices.CreateScope();
            var dc = scope.ServiceProvider.GetRequiredService<BarContext>();
            dc.Database.Migrate();
        }
    }
}