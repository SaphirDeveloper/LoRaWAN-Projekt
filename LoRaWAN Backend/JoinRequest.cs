﻿namespace LoRaWAN
{
    class JoinRequest : BackendPacket
    {

        public String SenderNSID;
        public String MacVersion;
        public String PhyPayload;
        public String DevEUI;
        public String DevAddr;
        public String DlSettings;
        public String RxDelay;
        public String CfList;

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