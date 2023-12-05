﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;
using LoRaWAN.PHYPayload;
using LoRaWAN.SemtechProtocol;
using Microsoft.Extensions.Configuration;


int port = 12345;
byte[] pullData = Utils.HexStringToByteArray("02010202a84041ffff1f8020");
byte[] pushData = Utils.HexStringToByteArray("02-07-D7-00-A8-40-41-FF-FF-1F-80-20-7B-22-72-78-70-6B-22-3A-5B-7B-22-74-6D-73-74-22-3A-39-36-34-31-38-35-37-35-36-2C-22-74-69-6D-65-22-3A-22-32-30-31-32-2D-30-31-2D-30-31-54-30-36-3A-31-34-3A-33-32-2E-38-32-36-37-36-33-5A-22-2C-22-63-68-61-6E-22-3A-33-2C-22-72-66-63-68-22-3A-30-2C-22-66-72-65-71-22-3A-38-36-37-2E-31-30-30-30-30-30-2C-22-73-74-61-74-22-3A-31-2C-22-6D-6F-64-75-22-3A-22-4C-4F-52-41-22-2C-22-64-61-74-72-22-3A-22-53-46-31-32-42-57-31-32-35-22-2C-22-63-6F-64-72-22-3A-22-34-2F-35-22-2C-22-6C-73-6E-72-22-3A-36-2E-38-2C-22-72-73-73-69-22-3A-2D-38-39-2C-22-73-69-7A-65-22-3A-32-33-2C-22-64-61-74-61-22-3A-22-59-50-67-76-30-41-47-4C-41-41-41-4E-62-4D-4F-55-55-6C-55-44-42-2F-38-41-41-57-5A-41-44-47-55-3D-22-7D-5D-7D".Replace("-", ""));
byte[] dataUp = Utils.HexStringToByteArray("02097a00a84041ffff1f80207b227278706b223a5b7b22746d7374223a313132363538313633362c226368616e223a312c2272666368223a312c2266726571223a3836382e3330303030302c2273746174223a312c226d6f6475223a224c4f5241222c2264617472223a22534631324257313235222c22636f6472223a22342f35222c226c736e72223a31322e352c2272737369223a2d36332c2273697a65223a32332c2264617461223a2251436b75415361414141414279566146353349752b767a6d77513d3d227d5d7d");
int serverPort = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port");
IPEndPoint end = new IPEndPoint(IPAddress.Any, port);

try
{
    Console.WriteLine($"Push Data JSON: {Encoding.UTF8.GetString(pushData[12..])}");
    Console.WriteLine($"Data Up JSON: {Encoding.UTF8.GetString(dataUp[12..])}");
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
