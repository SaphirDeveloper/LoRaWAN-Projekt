using System.Net;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;

namespace NetworkServer
{
    public class NetworkServer : Server
    {
        // Field
        private Thread _udpListnerThread;
        private UdpClient _udpClient;
        private IPEndPoint _groupEP;
        private int _port = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port");
        

        // Constructor
        public NetworkServer() : base(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["NetworkServerURL"])
        {
            _udpListnerThread = new Thread(AwaitUDPPacket);
            _groupEP = new IPEndPoint(IPAddress.Any, _port);
        }


        // Methods
        private void AwaitUDPPacket()
        {
            while (true)
            {
                // Await UDP packet
                byte[] bytes = _udpClient.Receive(ref _groupEP);

                // Process UDP packet
                Console.WriteLine("UDP packet received:");
                Console.WriteLine(Encoding.ASCII.GetString(bytes));
                Console.WriteLine();
            }
        }

        public override void ProcessPacket(string json)
        {
            Console.WriteLine(json);
            Console.WriteLine();
        }

        public override void Start()
        {
            base.Start();
            _udpClient = new UdpClient(_port);
            _udpListnerThread.Start();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            _udpClient.Close();
            _udpClient?.Dispose();
        }
    }
}
