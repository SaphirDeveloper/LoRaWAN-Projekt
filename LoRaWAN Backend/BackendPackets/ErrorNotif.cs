using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "messageType")]
    [JsonDerivedType(typeof(ErrorNotif), typeDiscriminator: "ErrorNotif")]
    public class ErrorNotif : BackendPacket
    {
        [DataMember(Name = "SenderNSID")]
        public string SenderNSID { get; set; }
        [DataMember(Name = "ReceiverNSID")]
        public string ReceiverNSID { get; set; }
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload { get; set; }
        [DataMember(Name = "Result")]
        public Result Result { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
