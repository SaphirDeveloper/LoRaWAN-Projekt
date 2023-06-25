using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    class BackendPacket : Packet
    {
        protected string ProtocolVersion;
        protected string SenderID;
        protected string ReceiverID;
        protected string MessageType;
        protected string SenderToken;
        protected string ReceiverToken;
        protected int TransactionID;

    }
}
