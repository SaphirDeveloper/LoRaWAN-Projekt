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

    }
}
