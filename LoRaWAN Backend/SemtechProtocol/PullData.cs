namespace LoRaWAN.SemtechProtocol
{
    public class PullData : SemtechPacket
    {
        public string GatewayMACaddress { get; internal set; }

        public PullAck CreatePullAck()
        {
            return new PullAck(Token);
        }
    }
}
