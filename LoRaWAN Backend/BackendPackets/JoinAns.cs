using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    public class JoinAns : BackendPacket
    {
        public string ReceiverNSID;
        public string PhyPayload;
        public Result Result;
        public float Lifetime;
        public KeyEnvelope AppSKey;
        public string SessionKeyID;
    }
}
