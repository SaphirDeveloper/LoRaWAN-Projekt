using System.Runtime;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "messageType")]
    [JsonDerivedType(typeof(JoinAns), typeDiscriminator: "JoinAns")]
    public class JoinAns : BackendPacket
    {
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload { get; set; }
        [DataMember(Name = "Result")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Result Result { get; set; }
        [DataMember(Name = "KeyEnvelope")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public KeyEnvelope AppSKey { get; set; }
        [DataMember(Name = "SessionKeyID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SessionKeyID { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        override
        protected string GetPacketInfo()
        {
            return $"PHYPayload: {PhyPayload}\r\nResult: {Result}\r\n" +
                   $"KeyEnvelope: {AppSKey}\r\nSessionKeyID: {SessionKeyID}";
        }
    }
}
