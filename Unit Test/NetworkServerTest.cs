using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Sockets;
using LoRaWAN.SemtechProtocol.Data;
using LoRaWAN.HTTP;
using LoRaWAN;
using Microsoft.AspNetCore.Hosting.Server;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace Unit_Test
{
    [TestClass]
    public class NetworkServerTest
    {
        private NetworkServer.NetworkServer _networkServer;
        private IWebHost _networkServerWebHost;

        [TestInitialize]
        public void StartNetworkServer()
        {
            // Start server
            _networkServer = new NetworkServer.NetworkServer();
            _networkServerWebHost = Backend.CreateWebHost(new string[0], _networkServer);
            _networkServerWebHost.StartAsync();
        }

        [TestCleanup]
        public void StopServer()
        {
            _networkServerWebHost.StopAsync().Wait();
            _networkServerWebHost.Dispose();
            _networkServer.Dispose();
        }

        [TestMethod]
        public void NetworkServerSingleJoinRequest()
        {
            byte[] joinRequest = Utils.HexStringToByteArray("02097a00a84041ffff1f80207b227278706b223a5b7b22746d7374223a313132363538313633362c226368616e223a312c2272666368223a312c2266726571223a3836382e3330303030302c2273746174223a312c226d6f6475223a224c4f5241222c2264617472223a22534631324257313235222c22636f6472223a22342f35222c226c736e72223a31322e352c2272737369223a2d36332c2273697a65223a32332c2264617461223a224141414141414141534c594549486451414178497467546f73776e4f6878673d227d5d7d");

            
        }

        [TestMethod]
        public void NetworkServerHTTPPostRequestTest()
        {
            // Send HTTP-POST-Request
            string networkServerURL = Appsettings.NetworkServerURL;
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
            udpClient.Connect("localhost", Appsettings.NetworkServerUDP_Port);
            udpClient.Send(message);
            udpClient.Close();
            udpClient.Dispose();
        }
    }
}
