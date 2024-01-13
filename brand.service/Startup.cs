using System.Reflection;
using BrandService.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddDbContext<BarContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("BarConnection"))
            );
            
            services.AddHealthChecks().AddDbContextCheck<BarContext>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
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