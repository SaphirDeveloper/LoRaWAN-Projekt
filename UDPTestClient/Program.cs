using System.Net;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;
using LoRaWAN.PHYPayload;
using LoRaWAN.SemtechProtocol;
using Microsoft.Extensions.Configuration;


int port = 12345;
byte[] pullData = Utils.HexStringToByteArray("02010202a84041ffff1f8020");
byte[] pushData = Utils.HexStringToByteArray("02097a00a84041ffff1f80207b227278706b223a5b7b22746d7374223a313132363538313633362c226368616e223a312c2272666368223a312c2266726571223a3836382e3330303030302c2273746174223a312c226d6f6475223a224c4f5241222c2264617472223a22534631324257313235222c22636f6472223a22342f35222c226c736e72223a31322e352c2272737369223a2d36332c2273697a65223a32332c2264617461223a224141414141414141534c594549486451414178497467546f73776e4f6878673d227d5d7d");
byte[] dataUp = Utils.HexStringToByteArray("02097a00a84041ffff1f80207b227278706b223a5b7b22746d7374223a313132363538313633362c226368616e223a312c2272666368223a312c2266726571223a3836382e3330303030302c2273746174223a312c226d6f6475223a224c4f5241222c2264617472223a22534631324257313235222c22636f6472223a22342f35222c226c736e72223a31322e352c2272737369223a2d36332c2273697a65223a32332c2264617461223a2251436b75415361414141414279566146353349752b767a6d77513d3d227d5d7d");
int serverPort = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port");
IPEndPoint end = new IPEndPoint(IPAddress.Any, port);

try
{
    Console.WriteLine($"Push Data JSON: {Encoding.UTF8.GetString(pushData[12..])}");
    Console.WriteLine();

    // Pull Data (No Response)
    Thread.Sleep(3000);
    UdpClient udpClient = new UdpClient(port);
    udpClient.Connect("localhost", serverPort);
    udpClient.Send(pullData);
    byte[] result = udpClient.Receive(ref end);
    Console.WriteLine($"Pull Ack : {BitConverter.ToString(result)}");

    // Push Data
    Thread.Sleep(3000);
    udpClient.Send(pushData);
    result = udpClient.Receive(ref end);
    Console.WriteLine($"Push Ack : {BitConverter.ToString(result)}");

    // Pull Data (With Response)
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
