using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "messageType")]
    [JsonDerivedType(typeof(JoinReq), typeDiscriminator: "JoinReq")]
    public class JoinReq : BackendPacket
    {
        [DataMember(Name = "SenderNSID")]
        public string SenderNSID { get; set; }
        [DataMember(Name = "MacVersion")]
        public string MacVersion { get; set; }
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload { get; set; }
        [DataMember(Name = "DevEUI")]
        public string DevEUI { get; set; }
        [DataMember(Name = "DevAddr")]
        public string DevAddr { get; set; }
        [DataMember(Name = "DLSettings")]
        public string DlSettings { get; set; }
        [DataMember(Name = "RxDelay")]
        public string RxDelay { get; set; }
        [DataMember(Name = "CFList")]
        public string CfList { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
