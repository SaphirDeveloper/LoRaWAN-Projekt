using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class ErrorNotif
    {

        private String SenderNSID;
        private String ReceiverNSID;
        private String PhyPayload;
        private Result Result;

        public ErrorNotif(String senderNSID, String receiverNSID, String phyPayload, Result result)
        {
            this.SenderNSID = senderNSID;
            this.ReceiverNSID = receiverNSID;
            this.PhyPayload = phyPayload;
            this.Result = result;
        }
    }
}
