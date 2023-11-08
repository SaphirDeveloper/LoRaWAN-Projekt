namespace LoRaWAN.SemtechProtocol.Data
{
    public class Stat
    {
        public string Time;
        public float Lati;
        public float Long;
        public float Ackr;
        public int Alti;
        public int Rxnb;
        public int Rxok;
        public int Rxfw;
        public int Dwnb;
        public int Txnb;

        public Stat(string time, float lati, float @long, int alti, int rxnb, int rxok, int rxfw, float ackr, int dwnb, int txnb)
        {
            Time = time;
            Lati = lati;
            Long = @long;
            Rxnb = rxnb;
            Rxok = rxok;
            Rxfw = rxfw;
            Ackr = ackr;
            Dwnb = dwnb;
            Txnb = txnb;


        }
    }
}
