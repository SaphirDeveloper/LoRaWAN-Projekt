using System.Text.Json.Serialization;
using LoRaWAN.SemtechProtocol.Data;
using Newtonsoft.Json.Linq;

namespace LoRaWAN.SemtechProtocol
{
    public class PushData : SemtechPacket
    {
        public string GatewayMACaddress { get; internal set; }
        public string JSON { get; internal set; }
        public Rxpk[] rxpks { get; internal set; }
        public Stat stat { get; internal set; }

        public PushAck CreatePushAck()
        {
            return new PushAck(Token);
        }
    }
}
