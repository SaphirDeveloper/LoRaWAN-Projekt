using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    class JoinRequest : BackendPacket
    {

        private String SenderNSID;
        private String MacVersion;
        private String PhyPayload;
        private String DevEUI;
        private String DevAddr;
        private String DlSettings;
        private String RxDelay;
        private String CfList;

        public JoinRequest(string senderNSID, String macVersion, String phyPayload, String devEUI, String devAddr, String dlSettings, String rxDelay, String cfList)
        {
            this.SenderNSID = senderNSID;
            this.MacVersion = macVersion;
            this.MacVersion = phyPayload;
            this.DevEUI = devEUI;
            this.DevAddr = devAddr;
            this.DlSettings = dlSettings;
            this.RxDelay = rxDelay;
            this.CfList = cfList;
        }


    }
}
