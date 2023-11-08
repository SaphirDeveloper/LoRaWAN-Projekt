namespace LoRaWAN.BackendPackets
{
    public class ErrorNotif
    {

        public string SenderNSID;
        public string ReceiverNSID;
        public string PhyPayload;
        public Result Result;

        public ErrorNotif(string senderNSID, string receiverNSID, string phyPayload, Result result)
        {
            SenderNSID = senderNSID;
            ReceiverNSID = receiverNSID;
            PhyPayload = phyPayload;
            Result = result;
        }
    }
}
