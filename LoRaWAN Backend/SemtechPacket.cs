using Newtonsoft.Json.Linq;
using System.Buffers.Text;
using System.Text;

namespace LoRaWAN
{
    public class SemtechPacket : Packet
    {
        public string ProtocolVersion;
        public string Token;
        public string Id;
        


        // decode Packet 
        // only needed when uplink (Gateway -> NS) or downlink (NS -> Gateway)
        public static string DecodePayload(byte[] byteStream)
        {

            // Extract the protocol version
            byte protocolVersion = byteStream[0];


            // Extract the random token
            byte[] randomToken = byteStream.Skip(1).Take(2).ToArray();
            // Convert to string
            string token = BitConverter.ToString(randomToken);


            // Extract the packet identifier
            byte packetIdentifier = byteStream[3];
            // Converting to string
            string id = System.Convert.ToString(packetIdentifier);


            // Extract the gateway unique identifier
            byte[] gatewayIdentifier = byteStream.Skip(4).Take(8).ToArray();
            // Convert to string
            string gatewayIdentifierString = BitConverter.ToString(gatewayIdentifier).Replace("-", ":");


            // Extract the JSON content from the byte stream
            byte[] jsonBytes = byteStream.Skip(12).Take(byteStream.Length - 12).ToArray();
            string jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

            Console.WriteLine(jsonString);

            JObject jsonObject = JObject.Parse(jsonString);

            if (jsonObject["rxpk"] != null)
            {
                Console.WriteLine("payload contains data");

                string data = jsonObject["rxpk"][0]["data"].Value<string>();
                if (data != null)
                {
                    byte[] decodedBase64 = Convert.FromBase64String(data);
                    string bitString = ByteArrayToBit(decodedBase64);

                    jsonObject["rxpk"][0]["data"] = bitString;
                    jsonString = jsonObject.ToString();
                }
            }
            

            return jsonString + "*" + id + "*" + token;
        }



        public byte[] EncodePayload()
        {

            // Create a byte list to store the encoded packet
            List<byte> encodedBytes = new List<byte>();

            // Convert the protocol version string to bytes
            byte[] protocolVersion = Encoding.UTF8.GetBytes(ProtocolVersion);
            // Add the protocol version to the byte list
            encodedBytes.AddRange(protocolVersion);

            // Convert the token string to bytes
            byte[] token = Encoding.UTF8.GetBytes(Token);
            // Add the token to the byte list
            encodedBytes.AddRange(token);

            // Convert the id string to bytes
            byte[] id = Encoding.UTF8.GetBytes(Id);
            encodedBytes.AddRange(id);

            // Convert to byte array 
            byte[] encodedPayload = encodedBytes.ToArray();

            if (encodedPayload.Length > 2408)
            {
                throw new ArgumentException("Encoded payload exceeds the maximum size limit.");
            }

            return encodedPayload;

        }


        /// Convert a hexadecimal string to a byte array
        public static byte[] ToByteArray(String hexString)
        {
            byte[] retval = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
                retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return retval;
        }

        /// Convert a byte array to a bit string
        public static string ByteArrayToBit(byte[] ba)
        {
            string bitString = "";
            foreach (byte b in ba)
            {
                // Convert the byte to its binary representation with leading zeros
                string binary = Convert.ToString(b, 2).PadLeft(8, '0');

                // Concatenate the binary representation to the bit string
                bitString += binary;

            }
            return bitString;
        }

        public static string HexStringToBinaryString(string hexString)
        {
            if (hexString == null)
                throw new ArgumentNullException(nameof(hexString));

            StringBuilder binaryStringBuilder = new StringBuilder(hexString.Length * 4 + (hexString.Length / 2)); // Account for the added spaces.

            int bitCount = 0; // Keep track of the number of bits added.

            foreach (char hexChar in hexString)
            {
                // Convert each hexadecimal character to its 4-bit binary representation.
                string binaryValue = Convert.ToString(Convert.ToInt32(hexChar.ToString(), 16), 2).PadLeft(4, '0');

                // Append the binary value to the result.
                binaryStringBuilder.Append(binaryValue);
                bitCount += 4; // Increment the bit count.

                // Insert a space after every 8 bits.
                if (bitCount % 8 == 0)
                {
                    binaryStringBuilder.Append(' ');
                }
            }

            return binaryStringBuilder.ToString();
        }
      
        public static string BinaryStringToHexString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                return binary;

            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... throw otherwise

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        /*
        public static void Main()
        {
            string hexString = "02097a00a84041ffff1f80207b2273746174223a7b2274696d65223a22323032332d30342d31382031303a33343a323720555443222c2272786e62223a302c2272786f6b223a302c2272786677223a302c2261636b72223a302e302c2264776e62223a302c2274786e62223a307d7d";




            byte[] retval = ToByteArray(hexString);
            Console.WriteLine((Encoding.ASCII.GetString(retval)));

            Packet packet = DecodePayload(retval);

            if (packet is SemtechPacket semtechPacket)
            {
                byte[] encodedPayload = semtechPacket.EncodePayload(semtechPacket);
                // Use the encoded payload as needed

                Console.WriteLine((Encoding.ASCII.GetString(encodedPayload)));


            }
            else
            {

                throw new ArgumentException("Invalid packet type. Expected SemtechPacket.");
                //---Implementation for BackendPacket

            }

        }
        */


        public static PHYpayload ExtractPhyPayload(string json)
        {
            PHYpayload phyPayload = null;
            JObject jsonObject = JObject.Parse(json);

            if (jsonObject["rxpk"] != null)
            {
                Console.WriteLine("payload contains data");

                string data = jsonObject["rxpk"][0]["data"].Value<string>();
                if (data != null)
                {
                    byte[] decodedBase64 = Convert.FromBase64String(data);
                    string bitString = ByteArrayToBit(decodedBase64);

                    int mhdrLength = 8;
                    int micLength = 32;
                    int macPayloadLength = bitString.Length - mhdrLength - micLength;

                    //building MHDR
                    string mhdrString = bitString.Substring(0, mhdrLength);
                    string major = mhdrString.Substring(0, 2); // Bits 0 and 1
                    string rfu = mhdrString.Substring(2, 3);   // Bits 2 to 4
                    string mType = mhdrString.Substring(5, 3); // Bits 5 to 7

                    MHDR mhdr = new MHDR(mType, rfu, major);
                    Console.WriteLine("mType: " + mhdr.MType + " rfu: " + mhdr.Rfu + " major: " + mhdr.Major);
                    //Console.ReadLine();

                    //building MACPayload
                    string macPayloadString = bitString.Substring(mhdrLength, macPayloadLength);
                    Console.WriteLine("MACPayload length: " + macPayloadString.Length + " and content: " + macPayloadString);
                    //Console.ReadLine();

                    //building MIC
                    string mic = bitString.Substring(bitString.Length - micLength, micLength);
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
                    else if (mType == "001")
                    {
                        // Join Accept
                        string appNonce = endianReverseBitString(macPayloadString.Substring(0, 24));
                        string netID = endianReverseBitString(macPayloadString.Substring (24, 24));
                        string devAddr = endianReverseBitString(macPayloadString.Substring(48, 32));
                        string dlSettings = macPayloadString.Substring(80, 8);
                        string rxDelay = macPayloadString.Substring(88, 8);
                        // TODO
                    }
                    else if (mType == "010")
                    {
                        // Unconfirmed Data Up
                    }




                    //Console.ReadLine();


                }

            }



            return phyPayload;
        }

        public static string endianReverseBitString(string bitString)
        {
            byte[] bytes = ToByteArray(BinaryStringToHexString(bitString));
            Array.Reverse(bytes);
            return ByteArrayToBit(bytes);
        }
    }
}