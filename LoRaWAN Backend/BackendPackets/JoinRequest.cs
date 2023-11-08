using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LoRaWAN.BackendPackets
{
    public class JoinRequest : BackendPacket
    {
        public string SenderNSID;
        public string MacVersion;
        public string PhyPayload;
        public string DevEUI;
        public string DevAddr;
        public string DlSettings;
        public string RxDelay;
        public string CfList;
    }
}
