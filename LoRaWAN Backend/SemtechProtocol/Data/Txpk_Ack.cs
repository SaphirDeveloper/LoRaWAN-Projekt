namespace LoRaWAN.SemtechProtocol.Data
{
    public class Txpk_Ack
    {
        public string Error;

        public Txpk_Ack(string error)
        {
            Error = error;
        }
    }
}
