using System.Runtime.Serialization;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    public class DataDown : BackendPacket
    {
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload { get; set; }
    }
}
