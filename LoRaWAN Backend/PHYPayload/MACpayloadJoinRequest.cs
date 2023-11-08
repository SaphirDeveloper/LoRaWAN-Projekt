namespace LoRaWAN.PHYPayload
{
    public class MACpayloadJoinRequest : MACpayload
    {
        public string AppEUI { get; internal set; }
        public string DevEUI { get; internal set; }
        public string DevNonce { get; internal set; }
    }
}
