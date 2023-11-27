namespace LoRaWAN.PHYPayload
{
    public class MACpayloadData : MACpayload
    {
        public FHDR Fhdr { get; internal set; }
        public string Fport { get; internal set; }
        public string FRMpayload { get; internal set; }
 

    }
}
