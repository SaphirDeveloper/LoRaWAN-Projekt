namespace LoRaWAN.PHYPayload
{
    public class MACpayloadData
    {
        private FHDR _fhdr;
        private string _fPort;
        private string _frmPayload;

        public MACpayloadData(FHDR _fhdr, string _fPort, string _frmPayload)
        {
            this._fhdr = _fhdr;
            this._fPort = _fPort;
            this._frmPayload = _frmPayload;
        }
    }
}
