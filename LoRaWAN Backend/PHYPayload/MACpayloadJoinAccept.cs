namespace LoRaWAN.PHYPayload
{
    public class MACpayloadJoinAccept : MACpayload
    {
        public string AppNonce { get; internal set; }
        public string NetID { get; internal set; }
        public string DevAddr { get; internal set; }
        public string DLSettings { get; internal set; }
        public string RxDelay { get; internal set; }
        public string CFList { get; internal set; }


    }
}
