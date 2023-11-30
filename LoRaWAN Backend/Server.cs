using LoRaWAN.BackendPackets;

namespace LoRaWAN
{
    public abstract class Server
    {
        // Field
        public string URL { get; internal set; }


        // Constructor
        public Server(string url)
        {
            URL = url;
        }

        // Abstract methods
        public abstract void ProcessPacket(BackendPacket backendPacket);

        public abstract string GetStatus();
    }
}
