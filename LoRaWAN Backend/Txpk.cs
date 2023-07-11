namespace LoRaWAN
{
    class Txpk : SemtechPacket
    {
        public bool Imme;
        public int Tmst;
        public long Tmms;
        public float Freq;
        public int Rfch;
        public int Powe;
        public string Modu;
        public string Datr;
        public string Codr;
        public int Fdev;
        public bool Ipol;
        public int Prea;
        public int Size;
        public string Data;
        public bool Ncrc;

        public Txpk(bool imme, int tmst, long tmms, float freq, int rfch, int powe, string modu,
            string datr, string codr, int fdev, bool ipol, int prea, int size, string data, bool ncrc)
        {
            this.Imme = imme;
            this.Tmst = tmst;
            this.Tmms = tmms;
            this.Freq = freq;
            this.Rfch = rfch;
            this.Powe = powe;
            this.Modu = modu;
            this.Datr = datr;
            this.Codr = codr;
            this.Fdev = fdev;
            this.Ipol = ipol;
            this.Prea = prea;
            this.Size = size;
            this.Data = data;
            this.Ncrc = ncrc;
        }
    }
}
