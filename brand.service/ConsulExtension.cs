using System.Net;
using System.Net.Sockets;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace BrandService
{
    public static class ConsulExtensions
    {
        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration.GetValue<string>("Consul:Host");
                consulConfig.Address = new Uri(address);
            }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
//            Thread.Sleep(60000);
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("ConsulExtensions");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            if (app.Properties["server.Features"] is not FeatureCollection features) return app;

            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses?.Addresses.First();

            Console.WriteLine($"address={address}");

            var hostname = Dns.GetHostName();
            var ip = Dns.GetHostEntry(hostname).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            int port = int.Parse(address?.Split(':')?.Last() ?? "80");            

            var registration = new AgentServiceRegistration()
            {
                ID = "BrandService",
                Name = "Brand Service",
                Address = $"{ip}",
                Port = port
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;
        }
    }
}