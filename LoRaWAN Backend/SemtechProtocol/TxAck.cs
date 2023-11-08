using LoRaWAN.SemtechProtocol.Data;

namespace LoRaWAN.SemtechProtocol
{
    public class TxAck : SemtechPacket
    {
        public string GatewayMACaddress { get; internal set; }
        public string JSON { get; internal set; }
        public Txpk_Ack txpk_ack { get; internal set; }
    }
}
