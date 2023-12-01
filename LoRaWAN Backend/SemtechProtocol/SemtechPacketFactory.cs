using LoRaWAN.SemtechProtocol.Data;
using Newtonsoft.Json.Linq;

namespace LoRaWAN.SemtechProtocol
{
    public static class SemtechPacketFactory
    {
        // Decode
        public static SemtechPacket DecodeSemtechPacket(byte[] bytes)
        {
            SemtechPacket packet;

            // Semtech identifier check 
            switch (bytes[3])
            {
                case 0:
                    // Push Data
                    packet = DecodePushData(bytes);
                    break;
                case 2:
                    // Pull data
                    packet = DecodePullData(bytes);
                    break;
                case 5:
                    // Tx Ack
                    packet = DecodeTxAck(bytes);
                    break;
                default:
                    throw new ArgumentException($"Unknown ID: {bytes[3]}");
            }
            // Protocol Version (0 byte)
            packet.ProtocolVersion = BitConverter.ToString(bytes[..1]).Replace("-", "");
            // Token (1-2 byte)
            packet.Token = BitConverter.ToString(bytes[1..3]).Replace("-", "");
            // Id (3 byte)
            packet.Id = BitConverter.ToString(bytes[3..4]).Replace("-", "");

            return packet;
        }

        public static PushData DecodePushData(byte[] bytes)
        {
            PushData packet = new PushData();
            // GatewayMACaddress (4-11 byte)
            packet.GatewayMACaddress = BitConverter.ToString(bytes[4..12]).Replace("-", "");
            // decode JSON
            packet.JSON = System.Text.Encoding.UTF8.GetString(bytes[12..]);
            JObject jObject = JObject.Parse(packet.JSON);
            // extracting all possible jObjects
            packet.rxpks = jObject["rxpk"]?.ToObject<Rxpk[]>();
            packet.stat = jObject["stat"]?.ToObject<Stat>();

            return packet;
        }

        public static PullData DecodePullData(byte[] bytes)
        {
            PullData packet = new PullData();
            // GatewayMACaddress (4-11 byte)
            packet.GatewayMACaddress = BitConverter.ToString(bytes[4..]).Replace("-", "");

            return packet;
        }

        public static TxAck DecodeTxAck(byte[] bytes)
        {
            TxAck packet = new TxAck();

            packet.GatewayMACaddress = BitConverter.ToString(bytes[4..12]).Replace("-", "");
            packet.JSON = System.Text.Encoding.UTF8.GetString(bytes[12..]);
            if (packet.JSON.Length > 0)
            {
                JObject jObject = JObject.Parse(packet.JSON);
                packet.txpk_ack = jObject["txpk_ack"]?.ToObject<Txpk_Ack>();
            }

            return packet;
        }

        // Create
        public static PullResp CreatePullResp(string token, string hexData)
        {
            Txpk txpk = new Txpk();
            txpk.Imme = true;
            txpk.Rfch = 0;
            txpk.Powe = 14;
            txpk.Freq = 867.5f;
            txpk.Modu = "LORA";
            txpk.Datr = "SF12BW125";
            txpk.Codr = "4/5";
            txpk.Ipol = true;
            txpk.Data = Convert.ToBase64String(Convert.FromHexString(hexData));
            txpk.Size = txpk.Data.Length / 2;
            return new PullResp(token, txpk);
        }

        // Extras
        public static string GenerateRandomToken()
        {
            Random random = new Random();
            byte[] bytes = new byte[2];
            random.NextBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
