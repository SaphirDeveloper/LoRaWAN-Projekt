using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoRaWAN;
using Microsoft.Extensions.Configuration;

namespace JoinServer
{
    public class JoinServer : Server
    {
        public JoinServer() : base(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["JoinServerURL"]) { }

        public override void ProcessPacket(string json)
        {
            throw new NotImplementedException();
        }

        public JoinAns ProcessJoinReq(JoinRequest req) 
        { 
            return new JoinAns(
                SemtechPacket.HexStringToBinaryString("473F81"),
                SemtechPacket.HexStringToBinaryString("000000"),
                SemtechPacket.HexStringToBinaryString("000000000"),
                SemtechPacket.HexStringToBinaryString("23"),
                SemtechPacket.HexStringToBinaryString("E0")
            );
        }

    }
}
