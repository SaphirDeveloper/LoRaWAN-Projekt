namespace LoRaWAN
{
    internal class KeyEnvelope
    {

        private String KekLabel;
        private String AesKey;

        public KeyEnvelope(String kekLabel, String aesKey)
        {
            this.KekLabel = kekLabel;
            this.AesKey = aesKey;
        }
    }
}
