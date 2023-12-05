using System.Text;
using LoRaWAN;
using LoRaWAN.HTTP;
using LoRaWAN.PHYPayload;
using LoRaWAN.SemtechProtocol.Data;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Unit_Test.Helper;

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

        [TestMethod]
        public void MultipleJoinsTest()
        {
            byte[] pushData = Utils.HexStringToByteArray("02-2B-AB-00-A8-40-41-FF-FF-1F-80-20-7B-22-72-78-70-6B-22-3A-5B-7B-22-74-6D-73-74-22-3A-37-34-31-37-30-32-38-34-34-2C-22-74-69-6D-65-22-3A-22-32-30-31-32-2D-30-31-2D-30-31-54-30-34-3A-35-39-3A-31-35-2E-33-36-30-35-37-35-5A-22-2C-22-63-68-61-6E-22-3A-36-2C-22-72-66-63-68-22-3A-30-2C-22-66-72-65-71-22-3A-38-36-37-2E-37-30-30-30-30-30-2C-22-73-74-61-74-22-3A-31-2C-22-6D-6F-64-75-22-3A-22-4C-4F-52-41-22-2C-22-64-61-74-72-22-3A-22-53-46-39-42-57-31-32-35-22-2C-22-63-6F-64-72-22-3A-22-34-2F-35-22-2C-22-6C-73-6E-72-22-3A-2D-36-2E-35-2C-22-72-73-73-69-22-3A-2D-31-30-32-2C-22-73-69-7A-65-22-3A-32-33-2C-22-64-61-74-61-22-3A-22-41-49-63-64-67-68-45-62-51-55-43-6F-68-78-32-43-45-64-74-42-51-4B-69-41-52-78-35-36-65-38-34-3D-22-7D-2C-7B-22-74-6D-73-74-22-3A-37-34-31-37-30-32-38-34-34-2C-22-74-69-6D-65-22-3A-22-32-30-31-32-2D-30-31-2D-30-31-54-30-34-3A-35-39-3A-31-35-2E-33-36-32-34-36-37-5A-22-2C-22-63-68-61-6E-22-3A-33-2C-22-72-66-63-68-22-3A-30-2C-22-66-72-65-71-22-3A-38-36-37-2E-31-30-30-30-30-30-2C-22-73-74-61-74-22-3A-31-2C-22-6D-6F-64-75-22-3A-22-4C-4F-52-41-22-2C-22-64-61-74-72-22-3A-22-53-46-39-42-57-31-32-35-22-2C-22-63-6F-64-72-22-3A-22-34-2F-35-22-2C-22-6C-73-6E-72-22-3A-2D-36-2E-30-2C-22-72-73-73-69-22-3A-2D-31-30-39-2C-22-73-69-7A-65-22-3A-32-33-2C-22-64-61-74-61-22-3A-22-41-49-63-64-67-68-45-62-51-55-43-6F-68-78-32-43-45-64-74-42-51-4B-69-41-52-78-35-36-65-38-34-3D-22-7D-2C-7B-22-74-6D-73-74-22-3A-37-34-31-37-30-32-38-34-34-2C-22-74-69-6D-65-22-3A-22-32-30-31-32-2D-30-31-2D-30-31-54-30-34-3A-35-39-3A-31-35-2E-33-36-34-33-39-38-5A-22-2C-22-63-68-61-6E-22-3A-31-2C-22-72-66-63-68-22-3A-31-2C-22-66-72-65-71-22-3A-38-36-38-2E-33-30-30-30-30-30-2C-22-73-74-61-74-22-3A-31-2C-22-6D-6F-64-75-22-3A-22-4C-4F-52-41-22-2C-22-64-61-74-72-22-3A-22-53-46-39-42-57-31-32-35-22-2C-22-63-6F-64-72-22-3A-22-34-2F-35-22-2C-22-6C-73-6E-72-22-3A-31-31-2E-32-2C-22-72-73-73-69-22-3A-2D-33-38-2C-22-73-69-7A-65-22-3A-32-33-2C-22-64-61-74-61-22-3A-22-41-49-63-64-67-68-45-62-51-55-43-6F-68-78-32-43-45-64-74-42-51-4B-69-41-52-78-35-36-65-38-34-3D-22-7D-5D-7D".Replace("-", ""));
            byte[] pullData = Utils.HexStringToByteArray("02010202a84041ffff1f8020");

            byte[] expectedPushAck = pushData[0..4];
            expectedPushAck[3] = 1;
            byte[] expectedPullAck = pullData[0..4];
            expectedPullAck[3] = 4;

            _udp.SendBytes(pushData);
            Thread.Sleep(1000);

            _udp.SendBytes(pullData);
            Thread.Sleep(1000);

            int count = 0;
            while (_udp.ReceivedPackets.Count < 5)
            {
                if (count++ >= 10)
                {
                    Assert.Fail("Waited to long for packet");
                }
                Thread.Sleep(1000);
            }

            CollectionAssert.AreEqual(expectedPushAck, _udp.ReceivedPackets.Dequeue());
            CollectionAssert.AreEqual(expectedPullAck, _udp.ReceivedPackets.Dequeue());

            for (int i = 0; i < 3; i++)
            {
                byte[] pullResp = _udp.ReceivedPackets.Dequeue();
                Assert.AreEqual(3, pullResp[3]);

                string json = Encoding.UTF8.GetString(pullResp[4..]);
                JObject jo = JObject.Parse(json);
                Txpk txpk = jo["txpk"].ToObject<Txpk>();
                PHYpayload payload = PHYpayloadFactory.DecodePHYPayloadFromBase64(txpk.Data);

                Assert.AreEqual("20", payload.MHDR.ToUpper());
            }
        }

        [TestMethod]
        public void DownlinkTest()
        {
            Random random = new Random();
            byte[] pushDataOneJoin = Utils.HexStringToByteArray("02097a00a84041ffff1f80207b227278706b223a5b7b22746d7374223a313132363538313633362c226368616e223a312c2272666368223a312c2266726571223a3836382e3330303030302c2273746174223a312c226d6f6475223a224c4f5241222c2264617472223a22534631324257313235222c22636f6472223a22342f35222c226c736e72223a31322e352c2272737369223a2d36332c2273697a65223a32332c2264617461223a224141414141414141534c594549486451414178497467546f73776e4f6878673d227d5d7d");
            byte[] pushDataThreeJoin = Utils.HexStringToByteArray("02-2B-AB-00-A8-40-41-FF-FF-1F-80-20-7B-22-72-78-70-6B-22-3A-5B-7B-22-74-6D-73-74-22-3A-37-34-31-37-30-32-38-34-34-2C-22-74-69-6D-65-22-3A-22-32-30-31-32-2D-30-31-2D-30-31-54-30-34-3A-35-39-3A-31-35-2E-33-36-30-35-37-35-5A-22-2C-22-63-68-61-6E-22-3A-36-2C-22-72-66-63-68-22-3A-30-2C-22-66-72-65-71-22-3A-38-36-37-2E-37-30-30-30-30-30-2C-22-73-74-61-74-22-3A-31-2C-22-6D-6F-64-75-22-3A-22-4C-4F-52-41-22-2C-22-64-61-74-72-22-3A-22-53-46-39-42-57-31-32-35-22-2C-22-63-6F-64-72-22-3A-22-34-2F-35-22-2C-22-6C-73-6E-72-22-3A-2D-36-2E-35-2C-22-72-73-73-69-22-3A-2D-31-30-32-2C-22-73-69-7A-65-22-3A-32-33-2C-22-64-61-74-61-22-3A-22-41-49-63-64-67-68-45-62-51-55-43-6F-68-78-32-43-45-64-74-42-51-4B-69-41-52-78-35-36-65-38-34-3D-22-7D-2C-7B-22-74-6D-73-74-22-3A-37-34-31-37-30-32-38-34-34-2C-22-74-69-6D-65-22-3A-22-32-30-31-32-2D-30-31-2D-30-31-54-30-34-3A-35-39-3A-31-35-2E-33-36-32-34-36-37-5A-22-2C-22-63-68-61-6E-22-3A-33-2C-22-72-66-63-68-22-3A-30-2C-22-66-72-65-71-22-3A-38-36-37-2E-31-30-30-30-30-30-2C-22-73-74-61-74-22-3A-31-2C-22-6D-6F-64-75-22-3A-22-4C-4F-52-41-22-2C-22-64-61-74-72-22-3A-22-53-46-39-42-57-31-32-35-22-2C-22-63-6F-64-72-22-3A-22-34-2F-35-22-2C-22-6C-73-6E-72-22-3A-2D-36-2E-30-2C-22-72-73-73-69-22-3A-2D-31-30-39-2C-22-73-69-7A-65-22-3A-32-33-2C-22-64-61-74-61-22-3A-22-41-49-63-64-67-68-45-62-51-55-43-6F-68-78-32-43-45-64-74-42-51-4B-69-41-52-78-35-36-65-38-34-3D-22-7D-2C-7B-22-74-6D-73-74-22-3A-37-34-31-37-30-32-38-34-34-2C-22-74-69-6D-65-22-3A-22-32-30-31-32-2D-30-31-2D-30-31-54-30-34-3A-35-39-3A-31-35-2E-33-36-34-33-39-38-5A-22-2C-22-63-68-61-6E-22-3A-31-2C-22-72-66-63-68-22-3A-31-2C-22-66-72-65-71-22-3A-38-36-38-2E-33-30-30-30-30-30-2C-22-73-74-61-74-22-3A-31-2C-22-6D-6F-64-75-22-3A-22-4C-4F-52-41-22-2C-22-64-61-74-72-22-3A-22-53-46-39-42-57-31-32-35-22-2C-22-63-6F-64-72-22-3A-22-34-2F-35-22-2C-22-6C-73-6E-72-22-3A-31-31-2E-32-2C-22-72-73-73-69-22-3A-2D-33-38-2C-22-73-69-7A-65-22-3A-32-33-2C-22-64-61-74-61-22-3A-22-41-49-63-64-67-68-45-62-51-55-43-6F-68-78-32-43-45-64-74-42-51-4B-69-41-52-78-35-36-65-38-34-3D-22-7D-5D-7D".Replace("-", ""));
            byte[] pullData = Utils.HexStringToByteArray("02010202a84041ffff1f8020");

            int pushDataCounter = 0;
            int pullDataCounter = 0;
            int joinReqCounter = 0;

            Thread pullDataThread = new Thread(() =>
            {
                try
                {
                    while (Thread.CurrentThread.IsAlive)
                    {
                        Thread.Sleep(1000);
                        _udp.SendBytes(pullData);
                        pullDataCounter++;
                    }
                }
                catch
                {
                    // Ignore Exception
                }
            });

            Thread pushDataThread = new Thread(() =>
            {
                try
                {
                    while (Thread.CurrentThread.IsAlive)
                    {
                        Thread.Sleep(random.Next(100, 1000));
                        if (random.NextDouble() < 0.5)
                        {
                            _udp.SendBytes(pushDataOneJoin);
                            joinReqCounter++;
                        }
                        else
                        {
                            _udp.SendBytes(pushDataThreeJoin);
                            joinReqCounter += 3;
                        }
                        pushDataCounter++;
                    }
                }
                catch 
                {
                    // Ignore Exception
                }
            });

            pullDataThread.Start();
            Thread.Sleep(2000);
            pushDataThread.Start();
            Thread.Sleep(30000);
            pushDataThread.Interrupt();
            pullDataThread.Interrupt();
            pushDataThread.Join();
            pullDataThread.Join();

            int pullAckCounter = 0;
            int pushAckCounter = 0;
            int pullRespCounter = 0;
            while (_udp.ReceivedPackets.Count > 0)
            {
                byte[] bytes = _udp.ReceivedPackets.Dequeue();

                switch(bytes[3])
                {
                    case 1:
                        pushAckCounter++;
                        break;
                    case 3:
                        pullRespCounter++;
                        break;
                    case 4:
                        pullAckCounter++;
                        break;
                }
            }

            Assert.AreEqual(pushDataCounter, pushAckCounter);
            Assert.AreEqual(pullDataCounter, pullAckCounter);
            Assert.AreEqual(joinReqCounter, pullRespCounter);
        }
    }
}
