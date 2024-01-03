using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "messageType")]
    [JsonDerivedType(typeof(AppSKeyAns), typeDiscriminator: "AppSKeyAns")]
    public class AppSKeyAns : BackendPacket
    {
        [DataMember(Name = "Result")]
        public Result Result { get; set; }
        [DataMember(Name = "DevEUI")]
        public string DevEUI { get; set; }
        [DataMember(Name = "AppSKey")]
        public KeyEnvelope AppSKey { get; set; }
        [DataMember(Name = "SessionalKeyID")]
        public string SessionKeyID { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
