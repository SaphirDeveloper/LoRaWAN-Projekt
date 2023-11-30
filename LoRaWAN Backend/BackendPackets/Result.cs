using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    public class Result
    {
        [DataMember(Name = "ResultCode")]
        public string ResultCode { get; set; }
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
