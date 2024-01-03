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
        [DataMember(Name = "MacVersion")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MacVersion { get; set; }
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload { get; set; }
        [DataMember(Name = "DevEUI")]
        public string DevEUI { get; set; }
        [DataMember(Name = "DevAddr")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DevAddr { get; set; }
        [DataMember(Name = "DLSettings")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DlSettings { get; set; }
        [DataMember(Name = "RxDelay")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RxDelay { get; set; }
        [DataMember(Name = "CFList")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CfList { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        override
        protected string GetPacketInfo()
        {
            return $"MacVersion: {MacVersion}\r\nPHYPayload: {PhyPayload}\r\n" +
                   $"DevEUI: {DevEUI}\r\nDevAddr: {DevAddr}\r\nDLSettings: {DlSettings}\r\nRxDelay: {RxDelay}\r\n" +
                   $"CfList: {CfList}";
        }
    }
}
