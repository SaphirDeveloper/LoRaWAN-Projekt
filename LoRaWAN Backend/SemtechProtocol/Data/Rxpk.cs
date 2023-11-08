namespace LoRaWAN.SemtechProtocol.Data
{
    public class Rxpk
    {
        public string Time;
        public long Tmms;
        public int Tmst;
        public float Freq;
        public int Chan;
        public int Rfch;
        public int Stat;
        public string Modu;
        public string Datr;
        public string Codr;
        public int Rssi;
        public float Lsnr;
        public int Size;
        public string Data;

        public Rxpk(string time, int tmms, long tmst, float freq, int chan, int rfch, int stat, string modu,
            string datr, string codr, int rssi, float lsnr, int size, string data)
        {
            Time = time;
            Tmst = tmms;
            Tmms = tmst;
            Freq = freq;
            Chan = chan;
            Rfch = rfch;
            Stat = stat;
            Modu = modu;
            Datr = datr;
            Codr = codr;
            Rssi = rssi;
            Lsnr = lsnr;
            Size = size;
            Data = data;
        }
    }
}
