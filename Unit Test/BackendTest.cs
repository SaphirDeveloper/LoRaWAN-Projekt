using LoRaWAN.HTTP;
using LoRaWAN;
using Unit_Test.Helper;
using Newtonsoft.Json.Linq;
using LoRaWAN.PHYPayload;
using System.Text;
using LoRaWAN.SemtechProtocol.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace Unit_Test
{
    [TestClass]
    public class BackendTest
    {
        private NetworkServer.NetworkServer _networkServer;
        private IWebHost _networkServerWebHost;
        private IWebHost _joinServerWebHost;
        private IWebHost _applicationServerWebHost;
        private UDP _udp;

        [TestInitialize]
        public void StartServer()
        {
            // Start servers
            _networkServer = new NetworkServer.NetworkServer();
            _networkServerWebHost = Backend.CreateWebHost(new string[0], _networkServer);
            _joinServerWebHost = Backend.CreateWebHost(new string[0], new JoinServer.JoinServer());
            _applicationServerWebHost = Backend.CreateWebHost(new string[0], new ApplicationServer.ApplicationServer());

            _networkServerWebHost.StartAsync();
            _joinServerWebHost.StartAsync();
            _applicationServerWebHost.StartAsync();

            Thread.Sleep(1000);

            // Start UDP Connection
            _udp = new UDP(12345);
            
            UDP.ProcessPacket ack = new UDP.ProcessPacket((byte[] packet) =>
            {
                if (packet[3] == 3)
                {
                    // Send TXPK_ACK
                    byte[] ack = packet[0..4].Concat(Utils.HexStringToByteArray("a84041ffff1f8020")).ToArray();
                    ack[3] = 5;
                    _udp.SendBytes(ack);
                }
            });
            _udp.onProcessPacket += ack;
            
            _udp.Start();
            _udp.Connect("localhost", Appsettings.NetworkServerUDP_Port);
        }

        [TestCleanup]
        public void StopServer()
        {
            _networkServerWebHost.StopAsync().Wait();
            _networkServerWebHost.Dispose();
            _joinServerWebHost.StopAsync().Wait();
            _joinServerWebHost.Dispose();
            _applicationServerWebHost.StopAsync().Wait();
            _applicationServerWebHost.Dispose();
            _udp.Stop();
            _networkServer.Dispose();
        }

        [TestMethod]
        public void ValidJoinTest()
        {
            byte[] pushDataJoinReq = Utils.HexStringToByteArray("02097a00a84041ffff1f80207b227278706b223a5b7b22746d7374223a313132363538313633362c226368616e223a312c2272666368223a312c2266726571223a3836382e3330303030302c2273746174223a312c226d6f6475223a224c4f5241222c2264617472223a22534631324257313235222c22636f6472223a22342f35222c226c736e72223a31322e352c2272737369223a2d36332c2273697a65223a32332c2264617461223a224141414141414141534c594549486451414178497467546f73776e4f6878673d227d5d7d");
            byte[] pullData = Utils.HexStringToByteArray("02010202a84041ffff1f8020");

            _udp.SendBytes(pushDataJoinReq);
            Thread.Sleep(1000);

            _udp.SendBytes(pullData);
            Thread.Sleep(1000);

            byte[] expectedPushAck = pushDataJoinReq[0..4];
            expectedPushAck[3] = 1;
            byte[] expectedPullAck = pullData[0..4];
            expectedPullAck[3] = 4;

            int count = 0;
            while (_udp.ReceivedPackets.Count < 3) 
            {
                if (count++ >= 10) 
                { 
                    Assert.Fail("Waited to long for packet");
                }
                Thread.Sleep(1000);
            }

            Assert.AreEqual(3, _udp.ReceivedPackets.Count);
            CollectionAssert.AreEqual(expectedPushAck, _udp.ReceivedPackets.Dequeue());
            CollectionAssert.AreEqual(expectedPullAck, _udp.ReceivedPackets.Dequeue());

            byte[] pullResp = _udp.ReceivedPackets.Dequeue();
            Assert.AreEqual(3, pullResp[3]);

            string json = Encoding.UTF8.GetString(pullResp[4..]);
            JObject jo = JObject.Parse(json);
            Txpk txpk = jo["txpk"].ToObject<Txpk>();
            PHYpayload payload = PHYpayloadFactory.DecodePHYPayloadFromBase64(txpk.Data);

            Assert.AreEqual("20", payload.MHDR.ToUpper());
        }
    }
}
