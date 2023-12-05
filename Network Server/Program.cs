using LoRaWAN;
using LoRaWAN.HTTP;

// Start Application Server
Server server = new NetworkServer.NetworkServer();
Backend.CreateWebHost(args, server).Run();
