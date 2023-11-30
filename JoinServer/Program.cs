using LoRaWAN;
using LoRaWAN.HTTP;

// Start Application Server
Server server = new JoinServer.JoinServer();
Backend.CreateAndStartWebHost(args, server);
