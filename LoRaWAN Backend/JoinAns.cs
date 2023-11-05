using Newtonsoft.Json;

namespace LoRaWAN
{
    [JsonObject]
    public class JoinAns : BackendPacket
    {

        private String ReceiverNSID;
        private String PhyPayload;
        private Result Result;
        private float Lifetime;
        private KeyEnvelope AppSKey;
        private String SessionKeyID;


        public JoinAns(string receiverNSID, String phyPayload, Result result, float Lifetime, KeyEnvelope appSKey, String sessionKeyID)
        {
            this.ReceiverNSID = receiverNSID;
            this.PhyPayload = phyPayload;
            this.Result = result;
            this.Lifetime = Lifetime;
            this.AppSKey = appSKey;
            this.SessionKeyID = sessionKeyID;
        }

        public string AppNonce;
        public string NetID;
        public string DevAddr;
        public string DLSettings;
        public string RxDelay;
        public string CFList;
        public JoinAns(string appNonce, string netID, string devAddr, string dlSettings, string rxDelay)
        {
            AppNonce = appNonce;
            NetID = netID;
            DevAddr = devAddr;
            DLSettings = dlSettings;
            RxDelay = rxDelay;
        }

        public JoinAns(string appNonce, string netID, string devAddr, string dlSettings, string rxDelay, string cFList)
        {
            AppNonce = appNonce;
            NetID = netID;
            DevAddr = devAddr;
            DLSettings = dlSettings;
            RxDelay = rxDelay;
            CFList = cFList;
        }
    }
}
