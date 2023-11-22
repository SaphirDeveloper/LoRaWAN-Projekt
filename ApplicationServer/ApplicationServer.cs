using LoRaWAN;

namespace ApplicationServer
{
    public class ApplicationServer : Server
    {

        public ApplicationServer() : base(Appsettings.ApplicationServerURL) { }

        public override void ProcessPacket(string json)
        {
            throw new NotImplementedException();
        }
    }
}
