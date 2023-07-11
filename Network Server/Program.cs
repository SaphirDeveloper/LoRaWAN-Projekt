using LoRaWAN;



// Start Test Server
Server server = new NetworkServer.NetworkServer();
server.Start();
server.WaitForShutdown();
