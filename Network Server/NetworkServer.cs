using LoRaWAN;

namespace NetworkServer
{
    public class NetworkServer : Server
    {

        // Constructor
        public NetworkServer() : base(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["NetworkServerURL"])
        {
            // Empty Constructor
        }


        // Method
        public override void ProcessPacket(string json)
        {
            Console.WriteLine(json);
            Console.WriteLine();
        }
    }
}
