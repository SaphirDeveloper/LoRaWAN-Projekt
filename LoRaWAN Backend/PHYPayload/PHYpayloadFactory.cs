using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LoRaWAN.PHYPayload
{
    public static class PHYpayloadFactory
    {
        // Decoding
        public static PHYpayload DecodePHYPayloadFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            string hex = BitConverter.ToString(bytes).Replace("-", "");
            return DecodePHYPayloadFromHex(hex);
        }

        public static PHYpayload DecodePHYPayloadFromHex(string hex)
        {
            PHYpayload payload = new PHYpayload();

            payload.Hex = hex;
            payload.MHDR = hex[..2]; // Extract the first 2 characters.
            payload.MACpayload = DecodeMACpayload(payload.MHDR, hex[2..^8]);
            payload.MIC = hex[^8..]; // Extract the last 8 characters.

            return payload;
        }

        public static MACpayload DecodeMACpayload(string mhdr, string hexMACpayload)
        {
            MACpayload macPayload;

            switch (mhdr)
            {
                case "00":
                    // Join Request
                    macPayload = DecodeMACpayloadJoinRequest(hexMACpayload);
                    break;
                case "20":
                    // Join Accept
                    macPayload = DecodeMACpayloadJoinAccept(hexMACpayload);
                    break;
                default:
                    throw new ArgumentException($"Invalid MHDR: {mhdr}");
            }

            macPayload.Hex = hexMACpayload;
            return macPayload;
        }

        public static MACpayloadJoinRequest DecodeMACpayloadJoinRequest(string hex)
        {
            MACpayloadJoinRequest payload = new MACpayloadJoinRequest();
            // Extract AppEUI, DevEUI, and DevNonce from the hex string
            payload.AppEUI = Utils.EndianReverseHexString(hex[..16]);
            payload.DevEUI = Utils.EndianReverseHexString(hex[16..32]);
            payload.DevNonce = Utils.EndianReverseHexString(hex[32..]);

            return payload;
        }

        public static MACpayloadJoinAccept DecodeMACpayloadJoinAccept(string hex)
        {
            MACpayloadJoinAccept payload = new MACpayloadJoinAccept();
            // Extract AppNonce, NetID, DevAddr, DLSettings, RxDelay, and CFList from the hex string
            payload.AppNonce = Utils.EndianReverseHexString(hex[..6]);
            payload.NetID = Utils.EndianReverseHexString(hex[6..12]);
            payload.DevAddr = Utils.EndianReverseHexString(hex[12..20]);
            payload.DLSettings = hex[20..22];
            payload.RxDelay = hex[22..24];
            payload.CFList = hex[24..];

            return payload;

        }

        // Create
        public static PHYpayload CreatePHYpayloadJoinAccept(string appNonce, string netID, string devAddr, string dlSettings, string rxDelay, string appKey)
        {
            PHYpayload phyPayload = new PHYpayload();
            MACpayloadJoinAccept macPayload = new MACpayloadJoinAccept();

            // Initialize the PHYpayload and MACpayload objects
            phyPayload.MACpayload = macPayload;
            // binary: 00100000 (001 = JoinAns) 
            phyPayload.MHDR = "20";
            macPayload.AppNonce = appNonce;
            macPayload.NetID = netID;
            macPayload.DevAddr = devAddr;
            macPayload.DLSettings = dlSettings;
            macPayload.RxDelay = rxDelay;

            // Build the hexadecimal representation of the payload
            StringBuilder sb = new StringBuilder();
            sb.Append(Utils.EndianReverseHexString(appNonce));
            sb.Append(Utils.EndianReverseHexString(netID));
            sb.Append(Utils.EndianReverseHexString(devAddr));
            sb.Append(Utils.EndianReverseHexString(dlSettings));
            sb.Append(Utils.EndianReverseHexString(rxDelay));
            macPayload.Hex = sb.ToString();
            sb.Insert(0, phyPayload.MHDR);

            // Calculate MIC
            byte[] cmac = Cryptography.AESCMAC(Utils.HexStringToByteArray(appKey), Utils.HexStringToByteArray(phyPayload.MHDR + macPayload.Hex));
            string mic = BitConverter.ToString(cmac[0..4]).Replace("-", "");
            phyPayload.MIC = mic;

            sb.Append(mic);
            phyPayload.Hex = sb.ToString();

            phyPayload.Hex = "20" + BitConverter.ToString(Cryptography.AESDecrypt(Utils.HexStringToByteArray(appKey), Utils.HexStringToByteArray(macPayload.Hex + mic))).Replace("-", "");

            return phyPayload;
        }

        public static PHYpayload CreatePHYpayloadJoinAccept(string appNonce, string netID, string devAddr, string dlSettings, string rxDelay, string cfList, string appKey)
        {
            PHYpayload phyPayload = new PHYpayload();
            MACpayloadJoinAccept macPayload = new MACpayloadJoinAccept();

            // Initialize the PHYpayload and MACpayload objects
            phyPayload.MACpayload = macPayload;
            // binary: 00100000 (001 = JoinAns) 
            phyPayload.MHDR = "20";
            macPayload.AppNonce = appNonce;
            macPayload.NetID = netID;
            macPayload.DevAddr = devAddr;
            macPayload.DLSettings = dlSettings;
            macPayload.RxDelay = rxDelay;
            macPayload.CFList = cfList;

            // Build the hexadecimal representation of the payload
            StringBuilder sb = new StringBuilder();
            sb.Append(Utils.EndianReverseHexString(appNonce));
            sb.Append(Utils.EndianReverseHexString(netID));
            sb.Append(Utils.EndianReverseHexString(devAddr));
            sb.Append(Utils.EndianReverseHexString(dlSettings));
            sb.Append(Utils.EndianReverseHexString(rxDelay));
            sb.Append(cfList);
            macPayload.Hex = sb.ToString();
            sb.Insert(0, phyPayload.MHDR);

            // Calculate MIC
            byte[] cmac = Cryptography.AESCMAC(Utils.HexStringToByteArray(appKey), Utils.HexStringToByteArray(phyPayload.MHDR + macPayload.Hex));
            string mic = BitConverter.ToString(cmac[0..4]).Replace("-", "");
            phyPayload.MIC = mic;

            sb.Append(mic);
            phyPayload.Hex = sb.ToString();

            phyPayload.Hex = "20" + BitConverter.ToString(Cryptography.AESDecrypt(Utils.HexStringToByteArray(appKey), Utils.HexStringToByteArray(macPayload.Hex + mic))).Replace("-", "");

            return phyPayload;
        }
    }
}
