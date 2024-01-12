using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Transactions;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using LoRaWAN.SemtechProtocol;
using LoRaWAN.SemtechProtocol.Data;
using Newtonsoft.Json;

namespace NetworkServer
{
    public class NetworkServer : Server, IDisposable
    {
        // Field
        private static readonly int JOIN_ACCEPT_DELAY1 = 5000000; // 5s
        private HttpClient _httpClient = new HttpClient();
        private Thread _udpListnerThread;
        private UdpClient _udpClient;
        private IPEndPoint _groupEP;
        private int _port = Appsettings.NetworkServerUDP_Port;
        private Queue<byte[]> pullResponesQueue = new Queue<byte[]>();
        private List<JoinReqAndRxpk> _openJoinReqs = new List<JoinReqAndRxpk>();
        private List<EndDevice> _endDevices = new List<EndDevice>();
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
                                    PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data);
                                    MACpayloadJoinRequest macPayload = (MACpayloadJoinRequest)phyPayload.MACpayload;
                                    joinRequest.PhyPayload = phyPayload.Hex;
                                    joinRequest.DevEUI = macPayload.DevEUI;
                                    JoinReqAndRxpk j = new JoinReqAndRxpk();

                                    EndDevice device = FindEndDeviceWithDevEUI(macPayload.DevEUI);
                                    if (device == null)
                                    {
                                        // Add new device
                                        device = new EndDevice();
                                        device.DevEUI = macPayload.DevEUI;
                                        
                                        // Generate new DevAddr
                                        string devAddr = BitConverter.ToString(BitConverter.GetBytes(_endDevices.Count).Reverse().ToArray()).Replace("-", "");

                                        device.DevAddr = devAddr;
                                        _endDevices.Add(device);
                                    }

                                    joinRequest.DevAddr = device.DevAddr;
                                    j.JoinReq = joinRequest;
                                    j.Rxpk = rxpk;
                                    _openJoinReqs.Add(j);

                                    string json = JsonConvert.SerializeObject(joinRequest);
                                    Logger.LogWriteSent(json, "NetworkServer", "JoinServer");
                                    _httpClient.PostAsJsonAsync(Appsettings.JoinServerURL, json).Wait();
                                }
                                if (mType == "010")
                                { // unconfirmed Dáta uplink
                                    DataUp dataUp = new DataUp();
                                    dataUp.TransactionID = ++_transactionCounter;
                                    dataUp.MessageType = "DataUp_unconf";
                                    PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromBase64(rxpk.Data);
                                    dataUp.PhyPayload = phyPayload.Hex;
                                    MACpayloadData data = (MACpayloadData)phyPayload.MACpayload;

                                    EndDevice device = FindEndDeviceWithDevAddr(data.Fhdr.DevAddr);
                                    if (device != null && device.AppSKey != null)
                                    {
                                        KeyEnvelope keyEnvelope = new KeyEnvelope();
                                        keyEnvelope.AesKey = device.AppSKey;
                                        dataUp.AppSKey = keyEnvelope;
                                    }

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
                JoinReqAndRxpk j = FindJoinReqWithTransactionID(packet.TransactionID);

                if (joinAns.Result.ResultCode != Result.RESULT_CODE_SUCCESS || j == null)
                {
                    // Join was not successful, ignore request
                    return;
                }

                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(joinAns.PhyPayload);

                EndDevice device = FindEndDeviceWithDevAddr(j.JoinReq.DevAddr);
                if (joinAns.AppSKey != null) device.AppSKey = joinAns.AppSKey.AesKey;
                if (joinAns.NwkSKey != null) device.NwkSKey = joinAns.NwkSKey.AesKey;

                PullResp pullResp = SemtechPacketFactory.CreatePullResp(SemtechPacketFactory.GenerateRandomToken(), phyPayload.Hex, j.Rxpk.Freq, j.Rxpk.Datr, j.Rxpk.Codr, j.Rxpk.Tmst + JOIN_ACCEPT_DELAY1);
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
            }
            else
            {
                Console.WriteLine($"Cannot process packet with type '{packet.MessageType}'");
            }
        }

        private JoinReqAndRxpk? FindJoinReqWithTransactionID(int transactionID)
        {
            foreach (JoinReqAndRxpk j in _openJoinReqs)
            {
                if (j.JoinReq.TransactionID == transactionID)
                {
                    _openJoinReqs.Remove(j);
                    return j;
                }
            }
            return null;
        }

        private EndDevice? FindEndDeviceWithDevAddr(string devAddr)
        {
            foreach (EndDevice device in _endDevices)
            {
                if (device.DevAddr == devAddr)
                {
                    return device;
                }
            }
            return null;
        }

        private EndDevice? FindEndDeviceWithDevEUI(string DevEUI)
        {
            foreach (EndDevice device in _endDevices)
            {
                if (device.DevEUI == DevEUI)
                {
                    return device;
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

            // End devices
            sb.AppendLine();
            sb.AppendLine("Enddevices:");
            foreach (EndDevice device in _endDevices)
            {
                sb.AppendLine($"  DevAddr: {device.DevAddr}");
                sb.AppendLine($"    DevEUI : {device.DevEUI}");
                sb.AppendLine($"    AppKey : {device.AppKey}");
                sb.AppendLine($"    AppSKey: {device.AppSKey}");
                sb.AppendLine($"    NwkSKey: {device.NwkSKey}");
            }

            return sb.ToString();
        }

        public void Dispose()
        {
            _udpClient.Close();
            _udpClient.Dispose();
        }



        private class JoinReqAndRxpk
        {
            public JoinReq JoinReq { get; set; }
            public Rxpk Rxpk { get; set; }
        }
    }
}
