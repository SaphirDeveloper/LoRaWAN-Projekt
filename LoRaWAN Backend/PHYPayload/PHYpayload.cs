using LoRaWAN.BackendPackets;

namespace LoRaWAN.PHYPayload
{

    public class PHYpayload
    {
        public string Hex {  get; internal set; }
        public string MHDR { get; internal set; }
        public MACpayload MACpayload { get; internal set; }
        public string MIC { get; internal set; }
    }
}
