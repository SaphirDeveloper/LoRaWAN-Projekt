using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using System.Text;

namespace ApplicationServer
{
    public class ApplicationServer : Server
    {
        private HttpClient _httpClient = new HttpClient();

        public ApplicationServer() : base(Appsettings.ApplicationServerURL) { }

        public override void ProcessPacket(BackendPacket packet)
        {
            // Check if the MessageType is "JoinReq"
            if (packet.MessageType.Equals("DataUp_unconf") || packet.MessageType.Equals("DataUp_conf"))
            {
                DataUp dataUp = (DataUp)packet;
                PHYpayload phyPayload = PHYpayloadFactory.DecodePHYPayloadFromHex(dataUp.PhyPayload);
                MACpayloadData macPayloadData = (MACpayloadData)phyPayload.MACpayload;

                if (macPayloadData.FRMpayload == null || dataUp.AppSKey == null) return;

                string frmPayloadDecrypted = Cryptography.DecryptFRMPayload(macPayloadData.FRMpayload, false, macPayloadData.Fhdr.DevAddr, "0000" + macPayloadData.Fhdr.FCnt, dataUp.AppSKey.AesKey);
                byte[] bytes = Utils.HexStringToByteArray(frmPayloadDecrypted);

                // Read FRMPayload of Dragino LHT52 sensor
                if (macPayloadData.Fport == "02" && bytes.Length == 11)
                {
                    float tempC_SHT = (((float)(bytes[0] << 24 >> 16 | bytes[1]) / 100));
                    float hum_SHT = (((float)(bytes[2] << 24 >> 16 | bytes[3]) / 10));
                    float tempC_DS = ((((float)(bytes[4] << 24 >> 16 | bytes[5])) / 100));

                    int ext = bytes[6];
                    int systimestamp = (bytes[7] << 24 | bytes[8] << 16 | bytes[9] << 8 | bytes[10]);

                    Console.WriteLine($"DevAddr: {macPayloadData.Fhdr.DevAddr}");
                    Console.WriteLine($"  FRMPayload (Decrypted): {frmPayloadDecrypted}");
                    Console.WriteLine($"  Temperature: {tempC_SHT}");
                    Console.WriteLine($"  Humidity: {hum_SHT}");
                    Console.WriteLine($"  External Temperature: {tempC_DS}");
                    Console.WriteLine($"  Ext: {ext}");
                    Console.WriteLine($"  Unix TimeStamp: {systimestamp}");
                }
                else if (macPayloadData.Fport == "05" && bytes.Length == 7)
                {
                    byte sensor_Model = bytes[0];
                    int firmware_Version = (bytes[1] << 8) | bytes[2];

                    string frquencyBand = bytes[3] switch
                    {
                        0x01 => "EU868",
                        0x02 => "US915",
                        0x03 => "IN865",
                        0x04 => "AU915",
                        0x05 => "KZ865",
                        0x06 => "RU864",
                        0x07 => "AS923",
                        0x08 => "AS923-1",
                        0x09 => "AS923-2",
                        0x0a => "AS923-3",
                        _ => $"Invalid Value: 0x{BitConverter.ToString(bytes[3..4])}",
                    };

                    byte sub_Band = bytes[4];
                    int bat_mV = bytes[5] << 8 | bytes[6];

                    Console.WriteLine($"DevAddr: {macPayloadData.Fhdr.DevAddr}");
                    Console.WriteLine($"  FRMPayload (Decrypted): {frmPayloadDecrypted}");
                    Console.WriteLine($"  Sensor Model: {sensor_Model}");
                    Console.WriteLine($"  Firmware Version: {firmware_Version}");
                    Console.WriteLine($"  Frequency Band: {frquencyBand}");
                    Console.WriteLine($"  Subband: {sub_Band}");
                    Console.WriteLine($"  BAT: {bat_mV}mV");
                }
                else
                {
                    Console.WriteLine("Invalid FPort and/or frame payload length");
                    Console.WriteLine($"  PHYPayload: {phyPayload.Hex}");
                    Console.WriteLine($"  DevAddr: {macPayloadData.Fhdr.DevAddr}");
                    Console.WriteLine($"  FPort: {macPayloadData.Fport}");
                    Console.WriteLine($"  FRMPayload (Decrypted): {frmPayloadDecrypted}");
                }
            } 
            else
            {
                Console.WriteLine($"Cannot process packet with type '{packet.MessageType}'");
            }
        }

        public override string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Application Server");
            return sb.ToString();
        }
    }
}
