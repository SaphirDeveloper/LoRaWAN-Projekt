using LoRaWAN;

// Start Test Server
Server server = new JoinServer.JoinServer();
server.Start();
server.WaitForShutdown();
