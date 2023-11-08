namespace LoRaWAN.SemtechProtocol
{
    public class PushAck : SemtechPacket
    {
        public PushAck(string token)
        {

            this.ProtocolVersion = "02";
            this.Token = token;
            this.Id = "01";
        }


        public override string ToString()
        {
            // Include the specific attributes of the PushAck class in the string representation
            return $"PushAck [ProtocolVersion: {ProtocolVersion}, Token: {Token}, Id: {Id}]";
        }

    }
}
