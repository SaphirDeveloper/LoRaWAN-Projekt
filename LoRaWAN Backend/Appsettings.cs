using Microsoft.Extensions.Configuration;

namespace LoRaWAN
{

    // TODO: Change it to a non-static class
    public static class Appsettings
    {
        public static int NetworkServerUDP_Port {get; private set;}
        public static string NetworkServerURL { get; private set; }
        public static string JoinServerURL { get; private set; }
        public static string ApplicationServerURL { get; private set; }
        

        static Appsettings()
        {
            IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            // TODO: Change default values
            NetworkServerUDP_Port = appsettings.GetValue("NetworkServerUDP_Port", 11000);
            NetworkServerURL = appsettings.GetValue("NetworkServerURL", "http://localhost:5100");
            JoinServerURL = appsettings.GetValue("JoinServerURL", "http://localhost:5200");
            ApplicationServerURL = appsettings.GetValue("ApplicationServerURL", "http://localhost:5300");
        }
    }
}
