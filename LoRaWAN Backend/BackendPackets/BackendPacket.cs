using LoRaWAN.BackendPackets.CustomJSONConverter;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    [SwaggerDiscriminator("messageType")]
    [SwaggerSubType(typeof(JoinReq), DiscriminatorValue = "JoinReq")]
    [SwaggerSubType(typeof(JoinAns), DiscriminatorValue = "JoinAns")]
    [SwaggerSubType(typeof(AppSKeyReq), DiscriminatorValue = "AppSKeyReq")]
    [SwaggerSubType(typeof(AppSKeyAns), DiscriminatorValue = "AppSKeyAns")]
    [SwaggerSubType(typeof(ErrorNotif), DiscriminatorValue = "ErrorNotif")]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "messageType")]
    [JsonDerivedType(typeof(JoinReq), typeDiscriminator: "JoinReq")]
    [JsonDerivedType(typeof(JoinAns), typeDiscriminator: "JoinAns")]
    [JsonDerivedType(typeof(AppSKeyReq), typeDiscriminator: "AppSKeyReq")]
    [JsonDerivedType(typeof(AppSKeyAns), typeDiscriminator: "AppSKeyAns")]
    [JsonDerivedType(typeof(ErrorNotif), typeDiscriminator: "ErrorNotif")]
    [Newtonsoft.Json.JsonConverter(typeof(BackendPacketJSONConverter))]
    public class BackendPacket
    {
        [DataMember(Name = "ProtocolVersion")]
        public string ProtocolVersion { get; set; }
        [DataMember(Name = "SenderID")]
        public string SenderID { get; set; }
        [DataMember(Name = "ReceiverID")]
        public string ReceiverID { get; set; }
        [DataMember(Name = "MessageType")]
        public string MessageType { get; set; }
        [DataMember(Name = "SenderToken")]
        public string SenderToken { get; set; }
        [DataMember(Name = "ReceiverToken")]
        public string ReceiverToken { get; set; }
        [DataMember(Name = "TransactionID")]
        public int TransactionID { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
