using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Sockets;
using LoRaWAN.SemtechProtocol.Data;
using LoRaWAN.HTTP;
using LoRaWAN;
using Microsoft.AspNetCore.Hosting.Server;

namespace Unit_Test
{
    [TestClass]
    public class NetworkServerTest
    {
        private Thread _serverThread;

        [TestInitialize]
        public void StartNetworkServer()
        {
            _serverThread = new Thread(() => Backend.CreateAndStartWebHost(new string[0], new NetworkServer.NetworkServer()));
            _serverThread.Start();
        }

        [TestMethod]
        public void NetworkServerHTTPPostRequestTest()
        {
            // Send HTTP-POST-Request
            string networkServerURL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["NetworkServerURL"];
            Stat stat = new Stat(DateTime.Now.ToString(), 46.24000f, 3.25230f, 145, 2, 2, 2, 100.0f, 2, 2);
            StringContent content = new StringContent(JsonConvert.SerializeObject(stat), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsync(networkServerURL, content).Result;
        }

        [TestMethod]
        public void NetworkServerUDPTest()
        {
            // Send UDP message
            byte[] message = Encoding.ASCII.GetBytes("Hello Network Server!");
            UdpClient udpClient = new UdpClient(12345);
            udpClient.Connect("localhost", new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port"));
            udpClient.Send(message);
            udpClient.Close();
            udpClient.Dispose();
        }
    }
}
