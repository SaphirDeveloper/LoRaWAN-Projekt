using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    class BackendPacket : Packet
    {
        public string ProtocolVersion;
        public string SenderID;
        public string ReceiverID;
        public string MessageType;
        public string SenderToken;
        public string ReceiverToken;
        public int TransactionID;

    }
}
