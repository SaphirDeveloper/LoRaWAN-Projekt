using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    public class DataUp : BackendPacket
    {
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload;
        [DataMember(Name = "AppSKey")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public KeyEnvelope AppSKey { get; set; }
    }
}
