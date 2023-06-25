using System;
using System.Net.Http;
using System.Threading;

namespace LoRaWAN
{
    class TestServer : Server
    {
        // Constructor
        public TestServer(string uri) : base(uri)
        {
            // Empty construcor
        }


        // Start
        public static void Main()
        {
            // Test, if the Server gets data from a HTTP-Post-Request
            Server server = new TestServer("http://localhost:8080/");
            new Thread(() => PostHttpRequest()).Start();
            new Thread(() => ShutdownServer(server)).Start();
            server.Start();
        }


        // Thread methods
        static void PostHttpRequest()
        {
            Thread.Sleep(3000);
            HttpClient client = new HttpClient();
            client.PostAsync("http://localhost:8080/", new StringContent("Hallo Welt!"));
        }

        static void ShutdownServer(Server server)
        {
            Thread.Sleep(6000);
            server.Shutdown();
        }


        // Method
        public override void ProcessPacket(string data)
        {
            throw new NotImplementedException();
        }
    }
}
