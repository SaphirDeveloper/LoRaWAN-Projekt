using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class PullAck : SemtechPacket
    {
        public PullAck(string token)
        {

            this.ProtocolVersion = "2";
            this.Token = token;
            this.Id = "0x04";
        }
    }
}
