using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    public class SemtechPacket : Packet
    {
        public string ProtocolVersion;
        public string Token;
        public string Id;



        //decode Packet 
        //only needed when uplink (Gateway -> NS) or downlink (NS -> Gateway)
        public Packet DecodePayload(byte[] byteStream)
        {

            // Extract the protocol version
            byte protocolVersion = byteStream[0];


            // Extract the random token
            byte[] randomToken = byteStream.Skip(1).Take(2).ToArray();
            //convert to string
            string Token = BitConverter.ToString(randomToken);


            // Extract the PUSH_DATA identifier
            byte pushDataIdentifier = byteStream[3];
            //converting to string
            string Id = System.Convert.ToString(byteStream[3]);


            // Extract the gateway unique identifier
            byte[] gatewayIdentifier = byteStream.Skip(4).Take(8).ToArray();
            // Convert to string
            string gatewayIdentifierString = BitConverter.ToString(gatewayIdentifier).Replace("-", ":");


            // Extract the JSON content from the byte stream
            byte[] jsonBytes = byteStream.Skip(12).Take(byteStream.Length - 13).ToArray();
            string JsonPayload = System.Text.Encoding.UTF8.GetString(jsonBytes);




            // Create the packet based on ID
            PacketFactory factory = new PacketFactory();

            Packet packet = factory.CreateSemtechPacket(Id, Token, JsonPayload);

            return packet;

            /*

            //---------Console Output---------//
            Console.WriteLine("protocolVersion: " + protocolVersion);
            Console.WriteLine("Token: " + Token);
            Console.WriteLine("ID: " + Id);
            Console.WriteLine("gatewayIdentifierString: " + gatewayIdentifierString);
            Console.WriteLine("json: " + JsonPayload);

            Console.WriteLine("Packet created: " + packet.GetType().Name);

            Console.WriteLine(packet.ToString());

            Console.ReadLine();

            */

        }

    }
}
