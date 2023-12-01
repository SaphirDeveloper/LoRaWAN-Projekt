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
            _udpClient = new UdpClient(_port);
            _udpListnerThread.Start();
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
                            string mType = Convert.ToString(tmp[0], 2).PadLeft(8, '0').Substring(0, 3);

                            if (mType == "000")
                            {
                                // create join request from the data inside the rxpk.Data
                                JoinReq joinRequest = new JoinReq();
                                joinRequest.MessageType = "JoinReq";
                                joinRequest.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                string json = JsonConvert.SerializeObject(joinRequest);
                                _httpClient.PostAsJsonAsync(Appsettings.JoinServerURL, json).Wait();
                            }
                            if (mType == "010")
                            { // unconfirmed Dáta uplink
                                DataUp dataUp = new DataUp();
                                dataUp.MessageType = "DataUp_unconf";
                                dataUp.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                string json = JsonConvert.SerializeObject(dataUp);
                                _httpClient.PostAsJsonAsync(Appsettings.ApplicationServerURL, json).Wait();
                            }
                            if (mType == "100")
                            { // confirmed Dáta uplink
                                DataUp dataUp = new DataUp();
                                dataUp.MessageType = "DataUp_conf";
                                dataUp.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                string json = JsonConvert.SerializeObject(dataUp);
                                _httpClient.PostAsJsonAsync(Appsettings.ApplicationServerURL, json).Wait();
                            }
                            if (mType == "011")
                            { // unconfirmed Data downlink
                                DataDown dataDown = new DataDown();
                                dataDown.MessageType = "DataDown_unconf";
                                dataDown.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                string json = JsonConvert.SerializeObject(dataDown);
                                _httpClient.PostAsJsonAsync(Appsettings.ApplicationServerURL, json).Wait();
                            }
                            if (mType == "101")
                            { // confirmed Data downlink
                                DataDown dataDown = new DataDown();
                                dataDown.MessageType = "DataDown_conf";
                                dataDown.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                string json = JsonConvert.SerializeObject(dataDown);
                                _httpClient.PostAsJsonAsync(Appsettings.ApplicationServerURL, json).Wait();
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
                        Logger.LogWriteSent(BitConverter.ToString(pullResp), "Network Server");
                    }
                }
                else if(packet.Id == "05")
                {
                    // Receive and process tx ack
                    Console.WriteLine("TxAck received");
                }
            }
        }
        
        public override void ProcessPacket(BackendPacket packet)
        {
            // Check if MessageType is "JoinAns"
            if (packet.MessageType.Equals("JoinAns"))
            {
                // Decode phyPayload from hex and create and enqueue a pull response
                JoinAns joinAns = (JoinAns)packet;
                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(joinAns.PhyPayload);
                PullResp pullResp = SemtechPacketFactory.CreatePullResp(SemtechPacketFactory.GenerateRandomToken(), phyPayload.Hex);
                pullResponesQueue.Enqueue(pullResp.EncodeSemtechPacket());
            }
            else
            {
                Console.WriteLine($"Cannot process packet with type '{packet.MessageType}'");
            }
        }

        public override string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Network Server");
            sb.AppendLine();
            sb.AppendLine($"Queue Length: {pullResponesQueue.Count}");
            return sb.ToString();
        }
    }
}
