using System.Net.Http;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;

namespace ApplicationServer
{
    public class ApplicationServer : Server
    {
        private HttpClient _httpClient = new HttpClient();

        public ApplicationServer() : base(Appsettings.ApplicationServerURL) { }

        public override void ProcessPacket(string json)
        {
            Logger.LogWrite(json, "Join Server");
            Console.WriteLine(json);
            JObject jObject = JObject.Parse(json);
            string messageType = jObject["MessageType"]?.Value<string>();

            // Check if the MessageType is "JoinReq"
            if (messageType.Equals("DataUp_unconf") || messageType.Equals("DataUp_conf"))
            {
                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex((string)jObject["PhyPayload"]);
                MACpayloadData macPayloadData = (MACpayloadData)phyPayload.MACpayload;
            } 
            else
            {
                Console.WriteLine("Cannot process JSON...");
            }
        }
    }
}
