using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JBHiFi.Samir.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
                await clientPolicyStore.SeedAsync();
            }

            await webHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
