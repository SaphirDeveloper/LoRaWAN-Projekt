using System.Net;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;
using Microsoft.Extensions.Configuration;

int port = 12345;
byte[] message = SemtechPacket.ToByteArray("02097a00a84041ffff1f80207b2273746174223a7b2274696d65223a22323032332d30342d31382031303a33343a323720555443222c2272786e62223a302c2272786f6b223a302c2272786677223a302c2261636b72223a302e302c2264776e62223a302c2274786e62223a307d7d");
int serverPort = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port");
IPEndPoint end = new IPEndPoint(IPAddress.Any, port);

while (true)
{
    try
    {
        Thread.Sleep(3000);
        UdpClient udpClient = new UdpClient(port);
        udpClient.Connect("localhost", serverPort);
        udpClient.Send(message);
        byte[] result = udpClient.Receive(ref end);

        Console.WriteLine(Encoding.ASCII.GetString(result));

        udpClient.Close();
        udpClient.Dispose();
    } 
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine();
    }
    
}
