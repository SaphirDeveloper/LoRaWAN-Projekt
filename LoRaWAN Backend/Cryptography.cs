using System.Security.Cryptography;

namespace LoRaWAN
{
    public static class Cryptography
    {
        private static byte[] AESEncrypt(byte[] key, byte[] iv, byte[] data, CipherMode mode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

                aes.Mode = mode;
                aes.Padding = PaddingMode.None;

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }

        private static byte[] AESDecrypt(byte[] key, byte[] iv, byte[] data, CipherMode mode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

                aes.Mode = mode;
                aes.Padding = PaddingMode.None;

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }

        private static byte[] Rol(byte[] b)
        {
            byte[] r = new byte[b.Length];
            byte carry = 0;

            for (int i = b.Length - 1; i >= 0; i--)
            {
                ushort u = (ushort)(b[i] << 1);
                r[i] = (byte)((u & 0xff) + carry);
                carry = (byte)((u & 0xff00) >> 8);
            }

            return r;
        }

        public static byte[] AESCMAC(byte[] key, byte[] data)
        {
            // SubKey generation
            // step 1, AES-128 with key K is applied to an all-zero input block.
            byte[] L = AESEncrypt(key, new byte[16], new byte[16], CipherMode.CBC);

            // step 2, K1 is derived through the following operation:
            byte[] FirstSubkey = Rol(L); //If the most significant bit of L is equal to 0, K1 is the left-shift of L by 1 bit.
            if ((L[0] & 0x80) == 0x80)
                FirstSubkey[15] ^= 0x87; // Otherwise, K1 is the exclusive-OR of const_Rb and the left-shift of L by 1 bit.

            // step 3, K2 is derived through the following operation:
            byte[] SecondSubkey = Rol(FirstSubkey); // If the most significant bit of K1 is equal to 0, K2 is the left-shift of K1 by 1 bit.
            if ((FirstSubkey[0] & 0x80) == 0x80)
                SecondSubkey[15] ^= 0x87; // Otherwise, K2 is the exclusive-OR of const_Rb and the left-shift of K1 by 1 bit.

            // MAC computing
            if (((data.Length != 0) && (data.Length % 16 == 0)) == true)
            {
                // If the size of the input message block is equal to a positive multiple of the block size (namely, 128 bits),
                // the last block shall be exclusive-OR'ed with K1 before processing
                for (int j = 0; j < FirstSubkey.Length; j++)
                    data[data.Length - 16 + j] ^= FirstSubkey[j];
            }
            else
            {
                // Otherwise, the last block shall be padded with 10^i
                byte[] padding = new byte[16 - data.Length % 16];
                padding[0] = 0x80;

                data = data.Concat<byte>(padding.AsEnumerable()).ToArray();

                // and exclusive-OR'ed with K2
                for (int j = 0; j < SecondSubkey.Length; j++)
                    data[data.Length - 16 + j] ^= SecondSubkey[j];
            }

            // The result of the previous process will be the input of the last encryption.
            byte[] encResult = AESEncrypt(key, new byte[16], data, CipherMode.CBC);

            byte[] HashValue = new byte[16];
            Array.Copy(encResult, encResult.Length - HashValue.Length, HashValue, 0, HashValue.Length);

            return HashValue;
        }

        public static (byte[] AppSKey, byte[] NwkSKey) GenerateSessionKeys(byte[] appKey, byte[] netId, byte[] appNonce, byte[] devNonce)
        {
            if (appKey.Length != 16) throw new ArgumentException("Expected an appKey with length 16");
            if (netId.Length != 3) throw new ArgumentException("Expected a netId with length 3");
            if (appNonce.Length != 3) throw new ArgumentException("Expected an appNonce with length 3");
            if (devNonce.Length != 2) throw new ArgumentException("Expected a devNonce with length 2");

            return (
                AppSKey: GenerateKey(appKey, appNonce, netId, devNonce, KeyType10.AppSKey),
                NwkSKey: GenerateKey(appKey, appNonce, netId, devNonce, KeyType10.NwkSKey)
            );
        }

        static byte[] GenerateKey(byte[] key, byte[] appNonce, byte[] netIdOrJoinEui, byte[] devNonce, KeyType10 keyType)
        {
            string keyNonceStr = keyType.ToString().Replace("AppSKey", "02").Replace("NwkSKey", "01") +
                                 BitConverter.ToString(appNonce.Reverse().ToArray()).Replace("-", "") +
                                 BitConverter.ToString(netIdOrJoinEui.Reverse().ToArray()).Replace("-", "") +
                                 BitConverter.ToString(devNonce.Reverse().ToArray()).Replace("-", "");

            keyNonceStr = keyNonceStr.PadRight(32, '0');

            byte[] keyNonce = StringToByteArray(keyNonceStr);
            byte[] encryptedKey = AESEncrypt(key, keyNonce);

            return encryptedKey;
        }

        static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] retArray = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                retArray[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return retArray;
        }

        enum KeyType10
        {
            AppSKey,
            NwkSKey
        }

        public static byte[] AESEncryptWithPadding(byte[] key, byte[] data)
        {
            if (data.Length % 16 > 0)
            {
                data = data.Concat(new byte[16 - data.Length % 16]).ToArray();
            }

            return AESEncrypt(key, new byte[16], data, CipherMode.ECB);
        }

        public static byte[] AESEncrypt(byte[] key, byte[] data)
        {
            return AESEncrypt(key, new byte[16], data, CipherMode.ECB);
        }

        public static byte[] AESDecrypt(byte[] key, byte[] data)
        {
            return AESDecrypt(key, new byte[16], data, CipherMode.ECB);
        }

      
    }
}
