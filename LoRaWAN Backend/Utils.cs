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
    }
}
