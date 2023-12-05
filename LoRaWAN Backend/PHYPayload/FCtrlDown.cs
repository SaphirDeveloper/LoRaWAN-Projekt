using LoRaWAN.SemtechProtocol;

namespace LoRaWAN.PHYPayload
{
    public class FCtrlDown
    {
        public bool Adr { get; internal set; }
        public bool Rfu { get; internal set; }
        public bool Ack { get; internal set; }
        public bool FPending { get; internal set; }
        public int FOptsLen { get; internal set; }
    }
}