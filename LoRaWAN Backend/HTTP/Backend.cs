﻿using Microsoft.AspNetCore;

namespace LoRaWAN.HTTP
{
    public class Backend
    {
        public static IWebHost CreateWebHost(string[] args, Server server) => 
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((services) => services.AddSingleton(server))
                .ConfigureLogging(config => {
                    config.ClearProviders();
                })
                .UseStartup<Startup>()
                .UseUrls(server.URL).Build();
    }
}
