using System.Buffers.Text;
using System.Linq.Expressions;
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
    public class NetworkServer : Server, IDisposable
    {
        // Field
        private HttpClient _httpClient = new HttpClient();
        private Thread _udpListnerThread;
        private UdpClient _udpClient;
        private IPEndPoint _groupEP;
        private int _port = Appsettings.NetworkServerUDP_Port;
        private Queue<byte[]> pullResponesQueue = new Queue<byte[]>();
        private List<JoinReq> _openJoinReqs = new List<JoinReq>();
        private bool _downlinkOpen = false;
        private int _pullDataPort = 0;
        private int _transactionCounter = 0;


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
            while (_udpClient.Client != null)
            {
                try
                {
                    // Await UDP packet
                    byte[] bytes = _udpClient.Receive(ref _groupEP);
                    Logger.LogWrite(BitConverter.ToString(bytes), "NetworkServer");
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
                                    joinRequest.TransactionID = ++_transactionCounter;
                                    joinRequest.MessageType = "JoinReq";
                                    joinRequest.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                    string json = JsonConvert.SerializeObject(joinRequest);
                                    _openJoinReqs.Add(joinRequest);
                                    Logger.LogWriteSent(json, "NetworkServer", "JoinServer");
                                    _httpClient.PostAsJsonAsync(Appsettings.JoinServerURL, json).Wait();
                                }
                                if (mType == "010")
                                { // unconfirmed Dáta uplink
                                    DataUp dataUp = new DataUp();
                                    dataUp.TransactionID = ++_transactionCounter;
                                    dataUp.MessageType = "DataUp_unconf";
                                    dataUp.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                    string json = JsonConvert.SerializeObject(dataUp);
                                    Logger.LogWriteSent(json, "NetworkServer", "ApplicationServer");
                                    _httpClient.PostAsJsonAsync(Appsettings.ApplicationServerURL, json).Wait();
                                }
                                if (mType == "100")
                                { // confirmed Dáta uplink
                                    DataUp dataUp = new DataUp();
                                    dataUp.TransactionID = ++_transactionCounter;
                                    dataUp.MessageType = "DataUp_conf";
                                    dataUp.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                    string json = JsonConvert.SerializeObject(dataUp);
                                    Logger.LogWriteSent(json, "NetworkServer", "ApplicationServer");
                                    _httpClient.PostAsJsonAsync(Appsettings.ApplicationServerURL, json).Wait();
                                }
                                if (mType == "011")
                                { // unconfirmed Data downlink
                                    DataDown dataDown = new DataDown();
                                    dataDown.TransactionID = ++_transactionCounter;
                                    dataDown.MessageType = "DataDown_unconf";
                                    dataDown.PhyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data).Hex;
                                    string json = JsonConvert.SerializeObject(dataDown);
                                    _httpClient.PostAsJsonAsync(Appsettings.ApplicationServerURL, json).Wait();
                                }
                                if (mType == "101")
                                { // confirmed Data downlink
                                    DataDown dataDown = new DataDown();
                                    dataDown.TransactionID = ++_transactionCounter;
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
                        _downlinkOpen = true;
                        _pullDataPort = _groupEP.Port;

                        // start dequeuing aslong pull response queue isn't empty 
                        if (pullResponesQueue.Count > 0)
                        {
                            byte[] pullResp = pullResponesQueue.Dequeue();
                            _groupEP.Port = _pullDataPort;
                            _udpClient.Send(pullResp, pullResp.Length, _groupEP);
                            Logger.LogWriteSent(BitConverter.ToString(pullResp), "NetworkServer", "Gateway");
                        }
                    }
                    else if (packet.Id == "05")
                    {
                        // Receive and process tx ack
                        Console.WriteLine("TxAck received");

                        // start dequeuing aslong pull response queue isn't empty 
                        if (pullResponesQueue.Count > 0)
                        {
                            byte[] pullResp = pullResponesQueue.Dequeue();
                            _groupEP.Port = _pullDataPort;
                            _udpClient.Send(pullResp, pullResp.Length, _groupEP);
                            Logger.LogWriteSent(BitConverter.ToString(pullResp), "NetworkServer", "Gateway");
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore Exception
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
                JoinReq joinReq = FindJoinReqWithTransactionID(packet.TransactionID);
                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(joinAns.PhyPayload);
                PullResp pullResp = SemtechPacketFactory.CreatePullResp(SemtechPacketFactory.GenerateRandomToken(), phyPayload.Hex);
                byte[] pullRespBytes = pullResp.EncodeSemtechPacket();

                if (_downlinkOpen)
                {
                    _groupEP.Port = _pullDataPort;
                    _udpClient.Send(pullRespBytes, pullRespBytes.Length, _groupEP);
                    Logger.LogWriteSent(BitConverter.ToString(pullRespBytes), "NetworkServer", "Gateway");
                }
                else
                {
                    pullResponesQueue.Enqueue(pullRespBytes);
                }

                _openJoinReqs.Remove(joinReq);
            }
            else
            {
                Console.WriteLine($"Cannot process packet with type '{packet.MessageType}'");
            }
        }

        public JoinReq? FindJoinReqWithTransactionID(int transactionID)
        {
            foreach (JoinReq packet in _openJoinReqs)
            {
                if (packet.TransactionID == transactionID)
                {
                    return packet;
                }
            }
            return null;
        }

        public override string GetStatus()
        {
            StringBuilder sb = new StringBuilder();

            // Server Name
            sb.AppendLine("Network Server");

            // Transaction Counter
            sb.AppendLine();
            sb.AppendLine($"Transaction Counter: {_transactionCounter}");

            // UDP Queue
            sb.AppendLine();
            sb.AppendLine($"UDP Sent Queue: {pullResponesQueue.Count}");
            foreach (byte[] bytes in pullResponesQueue)
            {
                sb.AppendLine($"[{BitConverter.ToString(bytes)}]");
            }

            // Open JoinReqs
            sb.AppendLine();
            sb.AppendLine($"Open Join Requests: {_openJoinReqs.Count}");
            foreach (JoinReq joinReq in _openJoinReqs)
            {
                sb.AppendLine(JsonConvert.SerializeObject(joinReq));
            }

            return sb.ToString();
        }

        public void Dispose()
        {
            _udpClient.Close();
            _udpClient.Dispose();
        }
    }
}
