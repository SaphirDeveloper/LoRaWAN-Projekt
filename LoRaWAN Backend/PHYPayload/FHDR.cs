namespace LoRaWAN.PHYPayload
{
    public class FHDR
    {
        public string DevAddr { get; internal set; }
        public FCtrlUp FCtrlUp { get; internal set; }
        public FCtrlDown FCtrlDown { get; internal set; }
        public string FCnt { get; internal set; }
        public string FOpts { get; internal set; }
    }
}
