namespace LoRaWAN
{

    public class PHYpayload
    {
        public MHDR mhdr { get; private set; }
        public MACpayload macPayload { get; private set; }
        public JoinRequest joinRequest { get; private set; }
        public JoinAns joinAns { get; private set; }
        public string mic { get; private set; }

        public PHYpayload(MHDR _mhdr, MACpayload _macPayload, string _mic)
        {
            this.mhdr = _mhdr;
            this.macPayload = _macPayload;
            this.mic = _mic;
        }

        public PHYpayload(MHDR _mhdr, JoinRequest _joinRequest, string _mic)
        {
            this.mhdr = _mhdr;
            this.joinRequest = _joinRequest;
            this.mic = _mic;
        }

        public PHYpayload(MHDR _mhdr, JoinAns _joinAns, string _mic)
        {
            this.mhdr = _mhdr;
            this.joinAns = _joinAns;
            this.mic = _mic;
        }

        
    }
}
