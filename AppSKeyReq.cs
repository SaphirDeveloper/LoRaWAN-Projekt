using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class AppSKeyReq
    {

        private String SenderNSID;
        private String DevEUI;
        private String DevAddr;
        private String SessionalKeyID;

        public AppSKeyReq(String senderNSID, String devEUI, String devAddr, String sessionalKeyID)
        {
            this.SenderNSID = senderNSID;
            this.DevEUI = devEUI;
            this.DevAddr = devAddr;
            this.SessionalKeyID = sessionalKeyID;
        }
    }
}
