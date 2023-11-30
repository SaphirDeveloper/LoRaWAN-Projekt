using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    public class KeyEnvelope
    {
        [DataMember(Name = "KEKLabel")]
        public string KekLabel { get; set; }
        [DataMember(Name = "AESKey")]
        public string AesKey { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
