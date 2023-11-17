using Newtonsoft.Json;

namespace LoRaWAN.SemtechProtocol.Data
{
    public class Txpk
    {
        [JsonProperty("imme")]
        public bool Imme;
        [JsonProperty("tmst")]
        public int Tmst;
        [JsonProperty("tmms")]
        public long Tmms;
        [JsonProperty("freq")]
        public float Freq;
        [JsonProperty("rfch")]
        public int Rfch;
        [JsonProperty("powe")]
        public int Powe;
        [JsonProperty("modu")]
        public string Modu;
        [JsonProperty("datr")]
        public string Datr;
        [JsonProperty("codr")]
        public string Codr;
        [JsonProperty("fdev")]
        public int Fdev;
        [JsonProperty("ipol")]
        public bool Ipol;
        [JsonProperty("prea")]
        public int Prea;
        [JsonProperty("size")]
        public int Size;
        [JsonProperty("data")]
        public string Data;
        [JsonProperty("ncrc")]
        public bool Ncrc;
    }
}
