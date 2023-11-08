namespace LoRaWAN.BackendPackets
{
    public class Result
    {

        public string ResultCode;
        public string Description;

        public Result(string resultCode, string description)
        {
            ResultCode = resultCode;
            Description = description;
        }
    }
}
