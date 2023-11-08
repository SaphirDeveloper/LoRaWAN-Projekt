namespace LoRaWAN.PHYPayload
{
    public class MHDR
    {
        public string MType { get; private set; }
        public string Rfu { get; private set; }
        public string Major { get; private set; }

        public MHDR(string MType, string Rfu, string Major)
        {
            this.MType = MType;
            this.Rfu = Rfu;
            this.Major = Major;

        }
    }
}
