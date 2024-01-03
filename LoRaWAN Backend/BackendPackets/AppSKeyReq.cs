using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "messageType")]
    [JsonDerivedType(typeof(AppSKeyReq), typeDiscriminator: "AppSKeyReq")]
    public class AppSKeyReq : BackendPacket
    {
        [DataMember(Name = "DevEUI")]
        public string DevEUI { get; set; }
        [DataMember(Name = "DevAddr")]
        public string DevAddr { get; set; }
        [DataMember(Name = "SessionalKeyID")]
        public string SessionalKeyID { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
