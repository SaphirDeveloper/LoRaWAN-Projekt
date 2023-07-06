using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    class JoinRequest : BackendPacket
    {

        private String _senderNSID;
        private String _macVersion;
        private String _phyPayload;
        private String _devEUI;
        private String _devAddr;
        private String _dlSettings;
        private String _rxDelay;
        private String _cfList;

        public JoinRequest()
        {

        }


    }
}
