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
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BrandService
{
    public class Startup
    {
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

            // We are using Conusl for service discovery
            // services.AddHttpClient("Beer").ConfigureHttpClient(c => c.BaseAddress = new Uri("http://beer"));

            services.AddOpenTelemetry().WithTracing( tracing => {
                tracing.AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = (httpContext) => !httpContext.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase);
                });
                tracing.AddHttpClientInstrumentation();
                tracing.AddEntityFrameworkCoreInstrumentation( t => t.SetDbStatementForText = true);
                //tracing.AddSqlClientInstrumentation(s => s.SetDbStatementForText = true);
                tracing.AddJaegerExporter(jaegerOptions =>
                {
                    jaegerOptions.AgentHost = "jaeger";
                    jaegerOptions.AgentPort = 6831;
                });
                tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("brand.service"));
            });

            services.AddSwaggerGen();
//            services.AddConsulConfig(Configuration);
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
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestIdMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers(); // Configurar enrutamiento para los controladores
            });

            app.UseSwagger();
            app.UseSwaggerUI();

//            app.UseConsul();

            using var scope = app.ApplicationServices.CreateScope();
            var dc = scope.ServiceProvider.GetRequiredService<BarContext>();
            dc.Database.Migrate();
        }
    }
}