using LoRaWAN;
using LoRaWAN.HTTP;

// Start Application Server
Server server = new ApplicationServer.ApplicationServer();
Backend.CreateAndStartWebHost(args, server);
