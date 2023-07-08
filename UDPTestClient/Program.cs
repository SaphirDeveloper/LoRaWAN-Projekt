using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;

int port = 12345;
byte[] message = Encoding.ASCII.GetBytes("Hello Network Server!");

while (true)
{
    try
    {
        Thread.Sleep(3000);
        UdpClient udpClient = new UdpClient(port);
        udpClient.Connect("localhost", new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port"));
        udpClient.Send(message);
        udpClient.Close();
        udpClient.Dispose();
    } 
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine();
    }
    
}
