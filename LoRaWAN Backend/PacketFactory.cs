using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    public class PacketFactory
    {

        public Packet packet;

        public Packet CreateSemtechPacket(String id, String token)
        {

            switch (id)
            {
                case "1":
                    return packet = new JoinRequest();
                case "2":
                    return packet = new JoinRequest();

            }
            return packet;

            /*
            Packet packet = id switch
            {
                "1" => new JoinRequest(), 
                "" => null

              

            };

            return packet;
            */
        }

        public Packet CreateBackendPacket(String protocolVersion, String senderID, String receiverID, String transactioID, String messageType, String senderToken, String receiverToken)
        {
            switch (messageType)
            {
                case "1":
                    return packet = new JoinRequest();
                case "2":

                    return packet = new JoinRequest();

            }
            return packet;
        }

    }
}
