namespace LoRaWAN.BackendPackets
{
    public class KeyEnvelope
    {

        public string KekLabel;
        public string AesKey;

        public KeyEnvelope(string kekLabel, string aesKey)
        {
            KekLabel = kekLabel;
            AesKey = aesKey;
        }
    }
}
