using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    class PushAck : SemtechPacket
    {
        public PushAck(string token)
        {

            this.ProtocolVersion = "2";
            this.Token = token;
            this.Id = "0x01";
        }


        public override string ToString()
        {
            // Include the specific attributes of the PushAck class in the string representation
            return $"PushAck [ProtocolVersion: {ProtocolVersion}, Token: {Token}, Id: {Id}]";
        }

    }
}
