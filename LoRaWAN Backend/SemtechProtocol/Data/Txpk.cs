using System.ComponentModel;
using Newtonsoft.Json;

namespace LoRaWAN.SemtechProtocol.Data
{
    public class Txpk
    {
        [JsonProperty("imme")]
        public bool Imme;
        [JsonProperty("tmst",DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public uint Tmst = 0;
        [JsonProperty("tmms", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(-1)]
        public long Tmms = -1;
        [JsonProperty("freq", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(-1)]
        public float Freq = -1;
        [JsonProperty("rfch", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(-1)]
        public int Rfch = -1;
        [JsonProperty("powe", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(-1)]
        public int Powe = -1;
        [JsonProperty("modu", NullValueHandling = NullValueHandling.Ignore)]
        public string Modu;
        [JsonProperty("datr", NullValueHandling = NullValueHandling.Ignore)]
        public string Datr;
        [JsonProperty("codr", NullValueHandling = NullValueHandling.Ignore)]
        public string Codr;
        [JsonProperty("fdev", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(-1)]
        public int Fdev = -1;
        [JsonProperty("ipol")]
        public bool Ipol;
        [JsonProperty("prea", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(-1)]
        public int Prea = -1;
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(-1)]
        public int Size = -1;
        [JsonProperty("data")]
        public string Data;
        [JsonProperty("ncrc", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Ncrc;
    }
}
