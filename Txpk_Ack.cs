using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class Txpk_Ack
    {
        public string Error;

        public Txpk_Ack(string error)
        {
            this.Error = error;
        }
    }
}
