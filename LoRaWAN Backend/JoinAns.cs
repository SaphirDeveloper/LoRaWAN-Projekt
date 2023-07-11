namespace LoRaWAN
{
    internal class JoinAns
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
    }
}
