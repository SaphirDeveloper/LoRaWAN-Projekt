using System.Security.Cryptography;
using System.Text;

namespace LoRaWAN
{
    public static class Utils
    {
        // Convert from Big Endian to Little Endian
        public static string EndianReverseHexString(string hex)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = hex.Length - 1; i >= 0; i -= 2)
            {
                sb.Append(hex[i - 1]).Append(hex[i]);
            }
            return sb.ToString();
        }

        // Convert hex string to byte array
        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex == null || hex.Length == 0) return new byte[0];

            byte[] retval = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                retval[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return retval;
        }

        public static bool GetBit(byte b, int bitNumber)
        {
            // Perform a bitwise AND operation to check if the specified bit is set
            // the bitmask created from the shift is used to perform a bool check on a specific bit  
            bool bit = (b & (1 << bitNumber - 1)) != 0;
            return bit;
        }

        private static readonly HashSet<string> usedNumbers = new HashSet<string>();
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static string GenerateUniqueRandomNumber(byte[] buffer)
        { 
            // Creates random number according to the buffer size
            rng.GetBytes(buffer);

            string randomNumber = BitConverter.ToString(buffer).Replace("-", "");

            // Creates a new number if the previouse number was allready generated once
            while (!usedNumbers.Add(randomNumber))
            {
                rng.GetBytes(buffer);
                randomNumber = BitConverter.ToString(buffer).Replace("-", "");
            }

            // Convert the integer to a hexadecimal string
            string hexString = randomNumber.ToString();
            return hexString;
        }

    }
}
