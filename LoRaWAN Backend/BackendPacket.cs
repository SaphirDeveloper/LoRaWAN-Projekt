namespace LoRaWAN
{
    public class BackendPacket : Packet
    {
        public string ProtocolVersion;
        public string SenderID;
        public string ReceiverID;
        public string MessageType;
        public string SenderToken;
        public string ReceiverToken;
        public int TransactionID;

    }
}
