using System.Net;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;
using LoRaWAN.PHYPayload;
using LoRaWAN.SemtechProtocol;
using Microsoft.Extensions.Configuration;


int port = 12345;
byte[] pullData = Utils.HexStringToByteArray("02107D02A84041FFFF1F8020");
byte[] pushData = Utils.HexStringToByteArray("0227D300A84041FFFF1F80207B227278706B223A5B7B22746D7374223A3132393635343639312C2274696D65223A22323031322D30312D30315430303A30323A34362E3839343331345A222C226368616E223A312C2272666368223A312C2266726571223A3836382E3330303030302C2273746174223A312C226D6F6475223A224C4F5241222C2264617472223A225346374257313235222C22636F6472223A22342F35222C226C736E72223A31302E302C2272737369223A2D33332C2273697A65223A32332C2264617461223A2241414142414141415155436F6741654859634242514B6745584F2B344F6E553D227D5D7D");
byte[] dataUp = Utils.HexStringToByteArray("028AC900A84041FFFF1F80207B227278706B223A5B7B22746D7374223A3133363638333438342C2274696D65223A22323031322D30312D30315430303A30323A35332E3934393238375A222C226368616E223A312C2272666368223A312C2266726571223A3836382E3330303030302C2273746174223A312C226D6F6475223A224C4F5241222C2264617472223A22534631324257313235222C22636F6472223A22342F35222C226C736E72223A31312E352C2272737369223A2D33352C2273697A65223A32312C2264617461223A2251414141414143424141414E42567870705148486D46716566597A71227D5D7D");
int serverPort = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port");
IPEndPoint end = new IPEndPoint(IPAddress.Any, port);

try
{
    Console.WriteLine($"Push Data JSON: {Encoding.UTF8.GetString(pushData[12..])}");
    Console.WriteLine($"Data Up JSON: {Encoding.UTF8.GetString(dataUp[12..])}");
    Console.WriteLine();

    // Connect to server
    Thread.Sleep(3000);
    UdpClient udpClient = new UdpClient(port);
    udpClient.Connect("localhost", serverPort);
    byte[] result;

    // Push Data
    Thread.Sleep(3000);
    udpClient.Send(pushData);
    result = udpClient.Receive(ref end);
    Console.WriteLine($"Push Ack : {BitConverter.ToString(result)}");

    // Pull Data
    Thread.Sleep(3000);
    udpClient.Send(pullData);
    result = udpClient.Receive(ref end);
    Console.WriteLine($"Pull Ack : {BitConverter.ToString(result)}");
    result = udpClient.Receive(ref end);
    Console.WriteLine($"Pull Resp: {BitConverter.ToString(result)}");
    Console.WriteLine();
    Console.WriteLine($"Pull Resp JSON: {Encoding.UTF8.GetString(result[4..])}");

    // Tx Ack
    Thread.Sleep(3000);
    string txAckHex = "02";
    txAckHex += BitConverter.ToString(result[1..3]).Replace("-", "");
    txAckHex += "05";
    txAckHex += "a84041ffff1f8020";
    byte[] txAck = Utils.HexStringToByteArray(txAckHex);
    udpClient.Send(txAck);

    // Data uplink
    Thread.Sleep(3000);
    udpClient.Send(dataUp);
    result = udpClient.Receive(ref end);
    Console.WriteLine($"Push Ack : {BitConverter.ToString(result)}");

    udpClient.Close();
    udpClient.Dispose();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine();
}

Console.WriteLine();
Console.WriteLine("End of the Programm...");
Console.ReadKey();
