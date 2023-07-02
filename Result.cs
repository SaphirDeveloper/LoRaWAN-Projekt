using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class Result
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
