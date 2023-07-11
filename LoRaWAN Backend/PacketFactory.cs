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
