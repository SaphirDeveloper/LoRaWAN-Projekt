namespace LoRaWAN
{
    internal class AppSKeyAns
    {

        private String ReceiverNSID;
        private Result Result;
        private String DevEUI;
        private KeyEnvelope AppSKey;
        private String SessionKeyID;

        public AppSKeyAns(String receiverNSID, Result result, String devEUI, KeyEnvelope appSKey, String sessionKeyID)
        {
            this.ReceiverNSID = receiverNSID;
            this.Result = result;
            this.DevEUI = devEUI;
            this.AppSKey = appSKey;
            this.SessionKeyID = sessionKeyID;
        }
    }
}
