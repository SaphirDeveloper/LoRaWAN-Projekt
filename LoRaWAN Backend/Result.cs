namespace LoRaWAN
{
    public class Result
    {

        private String ResultCode;
        private String Description;

        public Result(String resultCode, String description)
        {
            this.ResultCode = resultCode;
            this.Description = description;
        }
    }
}
