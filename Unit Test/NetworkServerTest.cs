

using LoRaWAN;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace Unit_Test
{
    [TestClass]
    public class NetworkServerTest
    {
        [TestMethod]
        public void NetworkServerHTTPPostRequestTest()
        {
            // Start server
            var server = new NetworkServer.NetworkServer();
            server.Start();

            // Send HTTP-POST-Request
            string networkServerURL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["NetworkServerURL"];
            Stat stat = new Stat(DateTime.Now.ToString(), 46.24000f, 3.25230f, 145, 2, 2, 2, 100.0f, 2, 2);
            StringContent content = new StringContent(JsonConvert.SerializeObject(stat), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsync(networkServerURL, content).Result;

            server.Shutdown();
        }

        [TestMethod]
        public void NetworkServerUDPTest()
        {
            // Start server
            var server = new NetworkServer.NetworkServer();
            server.Start();

            // Send UDP message
            byte[] message = Encoding.ASCII.GetBytes("Hello Network Server!");
            UdpClient udpClient = new UdpClient(12345);
            udpClient.Connect("localhost", new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port"));
            udpClient.Send(message);
            udpClient.Close();
            udpClient.Dispose();

            server.Shutdown();
        }
    }
}
