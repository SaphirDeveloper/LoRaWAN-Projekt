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
        public FCtrlDown(string hex)
        {
            byte byteValue = Convert.ToByte(hex, 16);

            Adr = Utils.GetBit(byteValue, 7);
            Rfu = Utils.GetBit(byteValue, 6);
            Ack = Utils.GetBit(byteValue, 5);
            FPending = Utils.GetBit(byteValue, 4);

        }
    }
}