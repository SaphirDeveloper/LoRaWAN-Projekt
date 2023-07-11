namespace LoRaWAN
{
    internal class Txpk_Ack
    {
        public string Error;

        public Txpk_Ack(string error)
        {
            this.Error = error;
        }
    }
}
