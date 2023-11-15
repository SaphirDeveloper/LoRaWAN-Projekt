using System.Buffers.Text;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using LoRaWAN.SemtechProtocol;
using LoRaWAN.SemtechProtocol.Data;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
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
                Logger.LogWrite(BitConverter.ToString(bytes), "Network Server");
                SemtechPacket packet = SemtechPacketFactory.DecodeSemtechPacket(bytes);
                
                // checking for push data packet
                if (packet.Id == "00")
                {
                    PushData pushData = (PushData)packet;
                    byte[] ack = pushData.CreatePushAck().EncodeSemtechPacket();

                    Console.WriteLine(BitConverter.ToString(ack));
                    _udpClient.Send(ack, ack.Length, _groupEP);

                    if (pushData.rxpks != null)
                    {
                        // Process rxpk in the push data packet
                        foreach (Rxpk rxpk in pushData.rxpks)
                        {
                            byte[] tmp = Convert.FromBase64String(rxpk.Data);
                            string mType = Convert.ToString(bytes[0], 2).PadLeft(8, '0').Substring(0, 3);

                            if (mType == "000")
                            {
                                // create join request from the data inside the rxpk.Data
                                JoinRequest joinRequest = new JoinRequest();
                                joinRequest.MessageType = "JoinReq";
                                joinRequest.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                string json = JsonConvert.SerializeObject(joinRequest);
                                _httpClient.PostAsJsonAsync(Appsettings.JoinServerURL, json).Wait();
                            }
                        }
                    }
                }
                // checking for pull data packet
                else if (packet.Id == "02")
                {
                    // Create and send pull acknowledgment
                    PullData pullData = (PullData)packet;
                    byte[] ack = pullData.CreatePullAck().EncodeSemtechPacket();
                    Console.WriteLine(BitConverter.ToString(ack));
                    _udpClient.Send(ack, ack.Length, _groupEP);

                    // start dequeuing aslong pull response queue isn't empty 
                    if (pullResponesQueue.Count > 0)
                    {
                        byte[] pullResp = pullResponesQueue.Dequeue();
                        _udpClient.Send(pullResp, pullResp.Length, _groupEP);
                    }
                }
                else if(packet.Id == "05")
                {
                    // Receive and process tx ack
                    Console.WriteLine("TxAck received");
                }
            }
        }
        
        public override void ProcessPacket(string json)
        {
            Logger.LogWrite(json, "Network Server");
            JObject jObject = JObject.Parse(json);
            // Check if MessageType is "JoinAns"
            if ((bool)(jObject["MessageType"]?.Value<string>().Equals("JoinAns")))
            {
                // Decode phyPayload from hex and create and enqueue a pull response
                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(jObject["PhyPayload"].Value<string>());
                PullResp pullResp = SemtechPacketFactory.CreatePullResp(SemtechPacketFactory.GenerateRandomToken(), phyPayload.Hex);
                pullResponesQueue.Enqueue(pullResp.EncodeSemtechPacket());
            }
            else
            {
                Console.WriteLine("Cannot process JSON...");
            }
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
