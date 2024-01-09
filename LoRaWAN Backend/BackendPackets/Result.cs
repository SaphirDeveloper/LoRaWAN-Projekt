using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    public class Result
    {
        public readonly static string RESULT_CODE_SUCCESS = "Success";
        public readonly static string RESULT_CODE_UNKNOWN_DEVEUI = "UnknownDevEUI";

        [DataMember(Name = "ResultCode")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ResultCode { get; set; }
        [DataMember(Name = "Description")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
