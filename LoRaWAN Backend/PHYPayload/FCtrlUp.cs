using LoRaWAN.SemtechProtocol;

namespace LoRaWAN.PHYPayload
{
    public class FCtrlUp
    {
        public bool Adr { get; internal set; }
        public bool AdrAckReq { get; internal set; }
        public bool Ack { get; internal set; }
        public bool ClassB { get; internal set; }
        public int FOptsLen { get; internal set; }
        public FCtrlUp(string hex)
        {
            byte byteValue = Convert.ToByte(hex, 16);

            Adr = Utils.GetBit(byteValue, 7);
            AdrAckReq = Utils.GetBit(byteValue, 6);
            Ack = Utils.GetBit(byteValue, 5);
            ClassB = Utils.GetBit(byteValue, 4);
            FOptsLen = byteValue & 0b00001111;
        }
    }
}