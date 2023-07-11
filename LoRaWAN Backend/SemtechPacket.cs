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
        public static Packet DecodePayload(byte[] byteStream)
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
            string id = System.Convert.ToString(byteStream[3]);


            // Extract the gateway unique identifier
            byte[] gatewayIdentifier = byteStream.Skip(4).Take(8).ToArray();
            // Convert to string
            string gatewayIdentifierString = BitConverter.ToString(gatewayIdentifier).Replace("-", ":");


            // Extract the JSON content from the byte stream
            byte[] jsonBytes = byteStream.Skip(12).Take(byteStream.Length - 13).ToArray();
            string jsonPayload = System.Text.Encoding.UTF8.GetString(jsonBytes);




            // Create the packet based on ID
            PacketFactory factory = new PacketFactory();

            Packet packet = factory.CreateSemtechPacket(id, token, jsonPayload);


            /*
            //---------Console Output---------//
            Console.WriteLine("protocolVersion: " + protocolVersion);
            Console.WriteLine("Token: " + token);
            Console.WriteLine("ID: " + id);
            Console.WriteLine("gatewayIdentifierString: " + gatewayIdentifierString);
            Console.WriteLine("json: " + jsonPayload);

            Console.WriteLine("Packet created: " + packet.GetType().Name);

            Console.WriteLine(packet.ToString());

            Console.ReadLine();
            */

            return packet;

        }



        public byte[] EncodePayload()
        {

            // Create a byte list to store the encoded packet
            List<byte> encodedBytes = new List<byte>();

            // Convert the protocol version string to bytes
            byte[] protocolVersion = Encoding.ASCII.GetBytes(ProtocolVersion);
            // Add the protocol version to the byte list
            encodedBytes.AddRange(protocolVersion);

            // Convert the token string to bytes
            byte[] token = Encoding.ASCII.GetBytes(Token);
            // Add the token to the byte list
            encodedBytes.AddRange(token);

            // Convert the id string to bytes
            byte[] id = Encoding.ASCII.GetBytes(Id);
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

    }
}