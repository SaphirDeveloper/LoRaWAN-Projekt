using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
        class PullResp : SemtechPacket
        {

        public string JsonPayload;

        public PullResp(string token, string jsonPayload)
        {

            this.ProtocolVersion = "2";
            this.Token = token;
            this.Id = "0x03";
            this.JsonPayload = jsonPayload;
        }

    }
}
