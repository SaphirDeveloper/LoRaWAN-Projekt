using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using LoRaWAN.SemtechProtocol;
using LoRaWAN.SemtechProtocol.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetworkServer
{
    public class NetworkServer : Server
    {
        // Field
        private HttpClient _httpClient = new HttpClient();
        private Thread _udpListnerThread;
        private UdpClient _udpClient;
        private IPEndPoint _groupEP;
        private int _port = Appsettings.NetworkServerUDP_Port;
        private Queue<byte[]> pullResponesQueue = new Queue<byte[]>();
        

        // Constructor
        public NetworkServer() : base(Appsettings.NetworkServerURL)
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

                SemtechPacket packet = SemtechPacketFactory.DecodeSemtechPacket(bytes);
                
                if (packet.Id == "00")
                {
                    PushData pushData = (PushData)packet;
                    byte[] ack = pushData.CreatePushAck().EncodeSemtechPacket();

                    Console.WriteLine(BitConverter.ToString(ack));
                    _udpClient.Send(ack, ack.Length, _groupEP);

                    foreach (Rxpk rxpk in pushData.rxpks)
                    {
                        JoinRequest joinRequest = new JoinRequest();
                        joinRequest.MessageType = "JoinReq";
                        joinRequest.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                        string json = JsonConvert.SerializeObject(joinRequest);
                        _httpClient.PostAsJsonAsync(Appsettings.JoinServerURL, json).Wait();
                    }
                }
                else if (packet.Id == "02")
                {
                    PullData pullData = (PullData)packet;
                    byte[] ack = pullData.CreatePullAck().EncodeSemtechPacket();
                    Console.WriteLine(BitConverter.ToString(ack));
                    _udpClient.Send(ack, ack.Length, _groupEP);

                    if (pullResponesQueue.Count > 0)
                    {
                        byte[] pullResp = pullResponesQueue.Dequeue();
                        _udpClient.Send(pullResp, pullResp.Length, _groupEP);

                        byte[] txAck = _udpClient.Receive(ref _groupEP);
                        SemtechPacket ackPacket = SemtechPacketFactory.DecodeSemtechPacket(txAck);
                        Console.WriteLine($"Packet received after PullResp: {ackPacket.Id}");
                    }
                }
            }
        }
        
        public override void ProcessPacket(string json)
        {
            JObject jObject = JObject.Parse(json);
            if ((bool)(jObject["MessageType"]?.Value<string>().Equals("JoinAns")))
            {
                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(jObject["PhyPayload"].Value<string>());
                PullResp pullResp = SemtechPacketFactory.CreatePullResp(SemtechPacketFactory.GenerateRandomToken(), phyPayload.Hex);
                pullResponesQueue.Enqueue(pullResp.EncodeSemtechPacket());
            }
            else
            {
                Console.WriteLine("Cannot process JSON...");
            }
        }

        public JoinRequest ProcessJoinReq(byte[] joinReq)
        {
            throw new NotImplementedException();
        }

        public byte[] ProcessJoinAns(JoinAns joinAns) 
        { 
            throw new NotImplementedException(); 
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
