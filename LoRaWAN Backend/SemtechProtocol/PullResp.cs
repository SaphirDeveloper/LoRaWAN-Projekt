using System.Text;
using LoRaWAN.SemtechProtocol.Data;
using Newtonsoft.Json;

namespace LoRaWAN.SemtechProtocol
{
    public class PullResp : SemtechPacket
    {

        public string JSON;
        public Txpk txpk;

        public PullResp(string token, Txpk txpk)
        {

            this.ProtocolVersion = "02";
            this.Token = token;
            this.Id = "03";
            this.txpk = txpk;
            // embedding the serialized txpk object inside the JSON string
            this.JSON = $"{{\"txpk\":{JsonConvert.SerializeObject(txpk)}}}"; 
        }

        public override byte[] EncodeSemtechPacket()
        {
            byte[] bytes = base.EncodeSemtechPacket();
            
            bytes = bytes.Concat(Encoding.ASCII.GetBytes(JSON)).ToArray();

            return bytes;
        }

    }
}
