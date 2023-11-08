using LoRaWAN.PHYPayload;
using Newtonsoft.Json.Linq;
using System.Buffers.Text;
using System.Text;

namespace LoRaWAN.SemtechProtocol
{
    public class SemtechPacket
    {
        public string ProtocolVersion { get; internal set; }
        public string Token { get; internal set; }
        public string Id { get; internal set; }

        public virtual byte[] EncodeSemtechPacket()
        {
            byte[] bytes = new byte[0];
            bytes = bytes.Concat(Utils.HexStringToByteArray(ProtocolVersion)).ToArray();
            bytes = bytes.Concat(Utils.HexStringToByteArray(Token)).ToArray();
            bytes = bytes.Concat(Utils.HexStringToByteArray(Id)).ToArray();
            return bytes;
        }
    }
}
