using Newtonsoft.Json.Linq;

namespace LoRaWAN
{
    public class PacketFactory
    {
        public Packet CreatePacket(string jsonpayload)
        {
            string[] temp = jsonpayload.Split('*');
            string json = temp[0];

            if (temp.Length > 1) {
                string id = temp[1];
                string token = temp[2];

                switch (id)
                {
                    //Push_Data received
                    case "0":
                        Console.WriteLine("PushData reseved and PushAck send");
                        return new PushAck(token);

                    //Pull_Data received
                    case "2":
                        return new PullAck(token);

                    //Pull_Resp
                    //send imidiatly after Pull_Ack if data has to be transfered back to the sensor
                    case "Beispielnummer":

                    //---not implemented yet---//

                    /*
                    return new PullResp(token, "JSON PAYLOAD");
                    */

                    //Tx_Ack reseived
                    case "5":

                    //---not implemented yet---//

                    default:
                        throw new ArgumentException("Unrecognized packet type.");

                }
            }
            else
            {
                return CreateBackendPacket(json);
            }
        }

        public PHYpayload CreateBackendPacket(string jsonpayload)
        {
            jsonpayload = jsonpayload.Split('*')[0];

            PHYpayload phyPayload = null;
            JObject jsonObject = JObject.Parse(jsonpayload);

            if (jsonObject["rxpk"] != null)
            {
                Console.WriteLine("payload contains data");

                string data = jsonObject["rxpk"][0]["data"].Value<string>();
                if (data != null)
                {
                    int mhdrLength = 8;
                    int micLength = 32;
                    int macPayloadLength = data.Length - mhdrLength - micLength;

                    //building MHDR
                    string mhdrString = data.Substring(0, mhdrLength);
                    string major = mhdrString.Substring(0, 2); // Bits 0 and 1
                    string rfu = mhdrString.Substring(2, 3);   // Bits 2 to 4
                    string mType = mhdrString.Substring(5, 3); // Bits 5 to 7

                    MHDR mhdr = new MHDR(mType, rfu, major);
                    Console.WriteLine("mType: " + mhdr.MType + " rfu: " + mhdr.Rfu + " major: " + mhdr.Major);
                    //Console.ReadLine();

                    //building MACPayload
                    string macPayloadString = data.Substring(mhdrLength, macPayloadLength);
                    Console.WriteLine("MACPayload length: " + macPayloadString.Length + " and content: " + macPayloadString);
                    //Console.ReadLine();

                    //building MIC
                    string mic = data.Substring(data.Length - micLength, micLength);
                    Console.WriteLine("MIC length: " + mic.Length + " and content: " + mic);

                    //message type (mType) check to know how to interpreted MACPayload bits 
                    if (mType == "000")
                    {
                        // Join Request
                        string appEui = endianReverseBitString(macPayloadString.Substring(0, 64));
                        string devEui = endianReverseBitString(macPayloadString.Substring(64, 64));
                        string devNonce = endianReverseBitString(macPayloadString.Substring(128, 16));

                        JoinRequest joinRequest = new JoinRequest(appEui, devEui, devNonce);
                        phyPayload = new PHYpayload(mhdr, joinRequest, mic);
                    }
                    else if (mType == "010")
                    {
                        // Unconfirmed Data Up
                    }
                    else if (mType == "100")
                    {
                        // Confirmed Data Up
                    }
                }

            }
            else if (jsonObject["txpk"] != null)
            {
                string data = jsonObject["txpk"]["data"].Value<string>();
                if (data != null)
                {

                }
                
            }

            return phyPayload;
        }

        public static string endianReverseBitString(string bitString)
        {
            byte[] bytes = SemtechPacket.ToByteArray(SemtechPacket.BinaryStringToHexString(bitString));
            Array.Reverse(bytes);
            return SemtechPacket.ByteArrayToBit(bytes);
        }
    }
}
