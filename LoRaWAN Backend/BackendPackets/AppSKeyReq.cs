namespace LoRaWAN.BackendPackets
{
    public class AppSKeyReq
    {
        public string SenderNSID;
        public string DevEUI;
        public string DevAddr;
        public string SessionalKeyID;

        public AppSKeyReq(string senderNSID, string devEUI, string devAddr, string sessionalKeyID)
        {
            SenderNSID = senderNSID;
            DevEUI = devEUI;
            DevAddr = devAddr;
            SessionalKeyID = sessionalKeyID;
        }
    }
}
