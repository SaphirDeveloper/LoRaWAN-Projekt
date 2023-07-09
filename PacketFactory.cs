using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace LoRaWAN
{
    public class PacketFactory
    {

        
        //Semtech Packet (Gateway-Network Server)
        public Packet CreateSemtechPacket(string id, string token, string jsonpayload)
        {

            switch (id)
            {   
                //Push_Data received
                case "0":
                    Console.WriteLine("PushData reseved and PushAck send");
                    return new PushAck(token);
                //Pull_Data received
                case "2":
                    return new PullAck(token);

                //wann sendet sever zu gateway Pull Resp packets?
                case "Beispielnummer":

                    return new PullResp(token, "JSON PAYLOAD");

                default:
                    throw new ArgumentException("Unrecognized packet type.");

            }


        }

        //Backend Packet (Network Server-Join Server- Application Server)
        public Packet CreateBackendPacket(String protocolVersion, String senderID, String receiverID, String transactioID, String messageType, String senderToken, String receiverToken)
        {
            switch (messageType)
            {
                case "1":
                    



                default:
                    throw new ArgumentException("Unrecognized packet type.");
            }

        }
    }
}
