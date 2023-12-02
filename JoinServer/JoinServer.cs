using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoinServer
{
    public class JoinServer : Server
    {
        private HttpClient _httpClient = new HttpClient();
        private List<EndDevice> _devices = EndDevice.ReadEndDeviceCSVList();

        public JoinServer() : base(Appsettings.JoinServerURL) { }

        public override void ProcessPacket(BackendPacket packet)
        {
            // Check if the MessageType is "JoinReq"
            if (packet.MessageType.Equals("JoinReq"))
            {
                JoinReq joinReq = (JoinReq)packet;
                PHYpayload joinReqPhyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(joinReq.PhyPayload);
                MACpayloadJoinRequest joinReqMacPayload = (MACpayloadJoinRequest)joinReqPhyPayload.MACpayload;
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
                    return;
                }


                // If it's a JoinReq, create a JoinAccept PHYpayload and send a JoinAns to the Network Server
                PHYpayload joinAnsPhyPayload = PHYpayloadFactory.CreatePHYpayloadJoinAccept("000000", "00000000", "94", "08", device.AppKey);
                MACpayloadJoinAccept joinAcceptMacPayload = (MACpayloadJoinAccept)joinAnsPhyPayload.MACpayload;
                JoinAns joinAns = new JoinAns();
                joinAns.MessageType = "JoinAns";
                joinAns.TransactionID = joinReq.TransactionID;
                joinAns.PhyPayload = joinAnsPhyPayload.Hex;
                _httpClient.PostAsJsonAsync(Appsettings.NetworkServerURL, JsonConvert.SerializeObject(joinAns)).Wait();

                var keys = Cryptography.GenerateSessionKeys(
                    Utils.HexStringToByteArray(device.AppKey),
                    Utils.HexStringToByteArray(joinAcceptMacPayload.NetID),
                    Utils.HexStringToByteArray(joinAcceptMacPayload.AppNonce),
                    Utils.HexStringToByteArray(joinReqMacPayload.DevNonce)
                );

                device.NwkSKey = BitConverter.ToString(keys.NwkSKey).Replace("-", "");
                device.AppSKey = BitConverter.ToString(keys.AppSKey).Replace("-", "");
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
                sb.AppendLine("  DevAddr: NOT SAVED YET");
                sb.AppendLine($"    DevEUI : {device.DevEUI}");
                sb.AppendLine($"    AppKey : {device.AppKey}");
                sb.AppendLine($"    AppSKey: {device.AppSKey}");
                sb.AppendLine($"    NwkSKey: {device.NwkSKey}");
            }
            return sb.ToString();
        }
    }
}
