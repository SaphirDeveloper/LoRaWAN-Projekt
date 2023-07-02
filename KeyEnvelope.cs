using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class KeyEnvelope
    {

        private String KekLabel;
        private String AesKey;

        public KeyEnvelope(String kekLabel, String aesKey)
        {
            this.KekLabel = kekLabel;
            this.AesKey = aesKey;
        }
    }
}
