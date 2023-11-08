namespace LoRaWAN.PHYPayload
{
    public class FHDR
    {
        private string _devAddr;
        private string _fCtrl;
        private string _fCnt;
        private string _fOpts;
        public FHDR(string _devAddr, string _fCtrl, string _fCnt, string _fOpts)
        {
            this._devAddr = _devAddr;
            this._fCtrl = _fCtrl;
            this._fCnt = _fCnt;
            this._fOpts = _fOpts;
        }
    }
}
