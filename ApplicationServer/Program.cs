using LoRaWAN;
using LoRaWAN.HTTP;

// Start Application Server
Server server = new ApplicationServer.ApplicationServer();
Backend.CreateWebHost(args, server).Run();
