using System.Net.Http;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;
using System.Text;

namespace ApplicationServer
{
    public class ApplicationServer : Server
    {
        private HttpClient _httpClient = new HttpClient();

        public ApplicationServer() : base(Appsettings.ApplicationServerURL) { }

        public override void ProcessPacket(BackendPacket packet)
        {
            // Check if the MessageType is "JoinReq"
            if (packet.MessageType.Equals("DataUp_unconf") || packet.MessageType.Equals("DataUp_conf"))
            {
                DataUp dataUp = (DataUp)packet;
                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(dataUp.PhyPayload);
                MACpayloadData macPayloadData = (MACpayloadData)phyPayload.MACpayload;
            } 
            else
            {
                Console.WriteLine($"Cannot process packet with type '{packet.MessageType}'");
            }
        }

        public override string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Application Server");
            return sb.ToString();
        }
    }
}
