namespace LoRaWAN
{
    public class KeyEnvelope
    {

        private string KekLabel;
        private string AesKey;

        public KeyEnvelope(string kekLabel, string aesKey)
        {
            this.KekLabel = kekLabel;
            this.AesKey = aesKey;
        }
    }
}
