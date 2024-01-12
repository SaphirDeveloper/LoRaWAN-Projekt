using System.Text;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using Newtonsoft.Json;

namespace JoinServer
{
    public class JoinServer : Server
    {
        private HttpClient _httpClient = new HttpClient();
        private List<EndDevice> _devices = ReadEndDeviceCSVList();

        public JoinServer() : base(Appsettings.JoinServerURL) { }

        public override void ProcessPacket(BackendPacket packet)
        {
            // Check if the MessageType is "JoinReq"
            if (packet.MessageType.Equals("JoinReq"))
            {
                // Decode Join Request
                JoinReq joinReq = (JoinReq)packet;
                PHYpayload joinReqPhyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(joinReq.PhyPayload);
                MACpayloadJoinRequest joinReqMacPayload = (MACpayloadJoinRequest)joinReqPhyPayload.MACpayload;

                // Create Join Answer
                JoinAns joinAns = new JoinAns();
                joinAns.MessageType = "JoinAns";
                joinAns.TransactionID = joinReq.TransactionID;
                joinAns.Result = new Result();
                string json;

                // Find device with DevEUI
                EndDevice device = null;
                foreach (EndDevice d in _devices)
                {
                    if (d.DevEUI.Equals(joinReqMacPayload.DevEUI))
                    {
                        device = d;
                        break;
                    }
                }

                if (device == null)
                {
                    // Device not found
                    joinAns.Result.ResultCode = Result.RESULT_CODE_UNKNOWN_DEVEUI;

                    // Send Join Answer
                    json = JsonConvert.SerializeObject(joinAns);
                    Logger.LogWriteSent(json, "JoinServer", "NetworkServer");
                    _httpClient.PostAsJsonAsync(Appsettings.NetworkServerURL, json).Wait();
                    return;
                }


                // If it's a JoinReq, create a JoinAccept PHYpayload and send a JoinAns to the Network Server
                PHYpayload joinAnsPhyPayload = PHYpayloadFactory.CreatePHYpayloadJoinAccept("000000", joinReq.DevAddr, "94", "08", device.AppKey);
                MACpayloadJoinAccept joinAcceptMacPayload = (MACpayloadJoinAccept)joinAnsPhyPayload.MACpayload;
                joinAns.PhyPayload = joinAnsPhyPayload.Hex;
                
                // Calculate Keys
                var keys = Cryptography.GenerateSessionKeys(
                    Utils.HexStringToByteArray(device.AppKey),
                    Utils.HexStringToByteArray(joinAcceptMacPayload.NetID),
                    Utils.HexStringToByteArray(joinAcceptMacPayload.AppNonce),
                    Utils.HexStringToByteArray(joinReqMacPayload.DevNonce)
                );

                device.NwkSKey = BitConverter.ToString(keys.NwkSKey).Replace("-", "");
                device.AppSKey = BitConverter.ToString(keys.AppSKey).Replace("-", "");

                // Send Keys with Join Answer
                KeyEnvelope envelopeNwkSKey = new KeyEnvelope();
                envelopeNwkSKey.AesKey = device.NwkSKey;
                KeyEnvelope envelopeAppSKey = new KeyEnvelope();
                envelopeAppSKey.AesKey = device.AppSKey;

                joinAns.NwkSKey = envelopeNwkSKey;
                joinAns.AppSKey = envelopeAppSKey;

                joinAns.Result.ResultCode = Result.RESULT_CODE_SUCCESS;

                // Send Join Answer
                json = JsonConvert.SerializeObject(joinAns);
                Logger.LogWriteSent(json, "JoinServer", "NetworkServer");
                _httpClient.PostAsJsonAsync(Appsettings.NetworkServerURL, json).Wait();

                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Cannot process packet with type '{packet.MessageType}'");
            }
        }

        public override string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Join Server");
            sb.AppendLine();
            sb.AppendLine("Enddevices:");
            foreach (EndDevice device in _devices)
            {
                sb.AppendLine($"  DevEUI : {device.DevEUI}");
                sb.AppendLine($"    DevAddr: NOT SAVED YET");
                sb.AppendLine($"    AppKey : {device.AppKey}");
                sb.AppendLine($"    AppSKey: {device.AppSKey}");
                sb.AppendLine($"    NwkSKey: {device.NwkSKey}");
            }
            return sb.ToString();
        }

        private static List<EndDevice> ReadEndDeviceCSVList()
        {
            var list = new List<EndDevice>();

            StreamReader reader = new StreamReader("./end_devices.csv");
            string[] head = reader.ReadLine().Split(';');
            int indexDevEUI = Array.IndexOf(head, "DevEUI");
            int indexAppKey = Array.IndexOf(head, "AppKey");

            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(';');
                EndDevice device = new EndDevice();
                device.DevEUI = row[indexDevEUI];
                device.AppKey = row[indexAppKey];
                list.Add(device);
            }

            return list;
        }
    }
}
