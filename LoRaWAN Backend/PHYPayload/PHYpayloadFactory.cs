using System.Text;

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
            payload.MHDR = hex[..2];
            payload.MACpayload = DecodeMACpayload(payload.MHDR, hex[2..^8]);
            payload.MIC = hex[^8..];

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

            payload.AppEUI = Utils.EndianReverseHexString(hex[..16]);
            payload.DevEUI = Utils.EndianReverseHexString(hex[16..32]);
            payload.DevNonce = Utils.EndianReverseHexString(hex[32..]);

            return payload;
        }

        public static MACpayloadJoinAccept DecodeMACpayloadJoinAccept(string hex)
        {
            MACpayloadJoinAccept payload = new MACpayloadJoinAccept();

            payload.AppNonce = Utils.EndianReverseHexString(hex[..6]);
            payload.NetID = Utils.EndianReverseHexString(hex[6..12]);
            payload.DevAddr = Utils.EndianReverseHexString(hex[12..20]);
            payload.DLSettings = hex[20..22];
            payload.RxDelay = hex[22..24];
            payload.CFList = hex[24..];

            return payload;

        }

        // Create
        public static PHYpayload CreatePHYpayloadJoinAccept(string appNonce, string netID, string devAddr, string dlSettings, string rxDelay, string mic)
        {
            PHYpayload phyPayload = new PHYpayload();
            MACpayloadJoinAccept macPayload = new MACpayloadJoinAccept();
            
            phyPayload.MACpayload = macPayload;
            phyPayload.MHDR = "20";
            macPayload.AppNonce = appNonce;
            macPayload.NetID = netID;
            macPayload.DevAddr = devAddr;
            macPayload.DLSettings = dlSettings;
            macPayload.RxDelay = rxDelay;
            phyPayload.MIC = mic;

            StringBuilder sb = new StringBuilder();
            sb.Append(Utils.EndianReverseHexString(appNonce));
            sb.Append(Utils.EndianReverseHexString(netID));
            sb.Append(Utils.EndianReverseHexString(devAddr));
            sb.Append(Utils.EndianReverseHexString(dlSettings));
            sb.Append(Utils.EndianReverseHexString(rxDelay));
            macPayload.Hex = sb.ToString();

            sb.Insert(0, phyPayload.MHDR);
            sb.Append(mic);
            phyPayload.Hex = sb.ToString();

            return phyPayload;
        }
    }
}
