using LoRaWAN;



// Start Test Server
Server server = new ApplicationServer.ApplicationServer();
server.Start();
server.WaitForShutdown();
