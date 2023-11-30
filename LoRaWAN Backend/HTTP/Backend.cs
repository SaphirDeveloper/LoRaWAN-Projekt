using Microsoft.AspNetCore;

namespace LoRaWAN.HTTP
{
    public class Backend
    {
        public static void CreateAndStartWebHost(string[] args, Server server)
        {
            Startup.Server = server;
            CreateWebHostBuilder(args).UseUrls(server.URL).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
