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

        public override void ProcessPacket(string json)
        {
            Logger.LogWrite(json, "Join Server");
            Console.WriteLine(json);
            JObject jObject = JObject.Parse(json);
            // Check if the MessageType is "JoinReq"
            if ((bool)(jObject["MessageType"]?.Value<string>().Equals("JoinReq")))
            {
                PHYpayload joinReqPhyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex((string)jObject["PhyPayload"]);
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
                PHYpayload joinAnsPhyPayload = PHYpayloadFactory.CreatePHYpayloadJoinAccept("E8B0C9", "000000", "00000000", "94", "08", device.AppKey);
                MACpayloadJoinAccept joinAcceptMacPayload = (MACpayloadJoinAccept)joinAnsPhyPayload.MACpayload;
                JoinAns joinAns = new JoinAns();
                joinAns.MessageType = "JoinAns";
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
                Console.WriteLine("Cannot process JSON...");
            }
        }
    }
}
