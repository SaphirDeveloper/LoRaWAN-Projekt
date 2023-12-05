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
        [DataMember(Name = "ReceiverNSID")]
        public string ReceiverNSID { get; set; }
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload { get; set; }
        [DataMember(Name = "Result")]
        public Result Result { get; set; }
        [DataMember(Name = "Lifetime")]
        public float Lifetime { get; set; }
        [DataMember(Name = "KeyEnvelope")]
        public KeyEnvelope AppSKey { get; set; }
        [DataMember(Name = "SessionKeyID")]
        public string SessionKeyID { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        override
        protected string GetPacketInfo()
        {
            return $"ReceiverNSID: {ReceiverNSID}\r\nPHYPayload: {PhyPayload}\r\nResult: {Result}\r\n" +
                   $"Lifetime: {Lifetime}\r\nKeyEnvelope: {AppSKey}\r\nSessionKeyID: {SessionKeyID}";
        }
    }
}
