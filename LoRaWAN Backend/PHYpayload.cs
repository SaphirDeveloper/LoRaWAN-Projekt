namespace LoRaWAN
{

    public class PHYpayload
    {
        private MHDR _mhdr;
        private MACpayload _macPayload;
        private JoinRequest _joinRequest;
        private JoinAns _joinAns;
        private string _mic;
        public PHYpayload(MHDR _mhdr, MACpayload _macPayload, string _mic)
        {
            this._mhdr = _mhdr;
            this._macPayload = _macPayload;
            this._mic = _mic;
        }

        public PHYpayload(MHDR _mhdr, JoinRequest _joinRequest, string _mic)
        {
            this._mhdr = _mhdr;
            this._joinRequest = _joinRequest;
            this._mic = _mic;
        }

        public PHYpayload(MHDR _mhdr, JoinAns _joinAns, string _mic)
        {
            this._mhdr = _mhdr;
            this._joinAns = _joinAns;
            this._mic = _mic;
        }
    }
}
