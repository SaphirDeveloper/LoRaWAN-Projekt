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
                throw new NotImplementedException();
            }
        }

    }
}
