﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using LoRaWAN;
using Newtonsoft.Json;

namespace NetworkServer
{
    public class NetworkServer : Server
    {
        // Field
        private HttpClient httpClient = new HttpClient();
        private PacketFactory _packetFactory = new PacketFactory();
        private Thread _udpListnerThread;
        private UdpClient _udpClient;
        private IPEndPoint _groupEP;
        private int _port = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<int>("NetworkServerUDP_Port");
        

        // Constructor
        public NetworkServer() : base(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["NetworkServerURL"])
        {
            _udpListnerThread = new Thread(AwaitUDPPacket);
            _groupEP = new IPEndPoint(IPAddress.Any, _port);
        }


        // Methods
        private void AwaitUDPPacket()
        {
            while (true)
            {
                // Await UDP packet
                byte[] bytes = _udpClient.Receive(ref _groupEP);

                string jsonString = SemtechPacket.DecodePayload(bytes);
                Packet packet = _packetFactory.CreatePacket(jsonString);
                
                if (packet is SemtechPacket semtechPacket)
                {
                    byte[] result = semtechPacket.EncodePayload();
                    Console.WriteLine(Encoding.ASCII.GetString(result));
                    _udpClient.Send(result, result.Length, _groupEP);
                }

                if (jsonString.ToLower().Contains("txpk") || jsonString.ToLower().Contains("rxpk"))
                {
                    PHYpayload backendPacket = _packetFactory.CreateBackendPacket(jsonString);
                    string joinURL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["JoinServerURL"];
                    string json = JsonConvert.SerializeObject(backendPacket);
                    httpClient.PostAsJsonAsync(joinURL, json).Wait();
                }
            }
        }
        
        public override void ProcessPacket(string json)
        {
            
            Console.WriteLine(json);
            Console.WriteLine();
        }

        public JoinRequest ProcessJoinReq(byte[] joinReq)
        {
            throw new NotImplementedException();
        }

        public byte[] ProcessJoinAns(JoinAns joinAns) 
        { 
            throw new NotImplementedException(); 
        }

        public override void Start()
        {
            base.Start();
            _udpClient = new UdpClient(_port);
            _udpListnerThread.Start();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            _udpClient.Close();
            _udpClient?.Dispose();
        }
    }
}
