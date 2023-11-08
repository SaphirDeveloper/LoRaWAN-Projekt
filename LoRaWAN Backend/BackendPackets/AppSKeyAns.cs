namespace LoRaWAN.BackendPackets
{
    public class AppSKeyAns
    {
        public string ReceiverNSID;
        public Result Result;
        public string DevEUI;
        public KeyEnvelope AppSKey;
        public string SessionKeyID;

        public AppSKeyAns(string receiverNSID, Result result, string devEUI, KeyEnvelope appSKey, string sessionKeyID)
        {
            ReceiverNSID = receiverNSID;
            Result = result;
            DevEUI = devEUI;
            AppSKey = appSKey;
            SessionKeyID = sessionKeyID;
        }
    }
}
