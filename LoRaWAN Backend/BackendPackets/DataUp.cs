using System.Runtime.Serialization;

namespace LoRaWAN.BackendPackets
{
    [DataContract]
    public class DataUp : BackendPacket
    {
        [DataMember(Name = "PHYPayload")]
        public string PhyPayload;
    }
}
