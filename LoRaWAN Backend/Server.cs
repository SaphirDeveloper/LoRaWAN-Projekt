using System;
using System.Net;
using System.Threading;

namespace LoRaWAN
{
    abstract class Server
    {
        // Field
        private HttpListener _httpListner = new HttpListener();


        // Constructor
        public Server(string uri)
        {
            // Set the URI of the server
            _httpListner.Prefixes.Add(uri);
        }


        // Thread method
        static void ProcessHttpRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            ShowRequestData(request);

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Close();
        }


        // Methods
        public static void ShowRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return;
            }

            // Prepare to read the request
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

            // Show request metadata
            if (request.ContentType != null)
            {
                Console.WriteLine("Client data content type {0}", request.ContentType);
            }
            Console.WriteLine("Client data content length {0}", request.ContentLength64);

            // Show request data
            Console.WriteLine("Start of client data:");
            Console.WriteLine(reader.ReadToEnd());
            Console.WriteLine("End of client data:");

            // Close
            body.Close();
            reader.Close();

            // If you are finished with the request, it should be closed also.
        }

        public void Start()
        {
            _httpListner.Start();
            Console.WriteLine("Server started...");

            AwaitPacket();
        }

        public void Shutdown()
        {
            _httpListner.Stop();
        }

        public void AwaitPacket()
        {
            while (_httpListner.IsListening)
            {
                try
                {
                    // Wait for request
                    HttpListenerContext context = _httpListner.GetContext();

                    // Request received, process request in a new thread
                    new Thread(() => ProcessHttpRequest(context)).Start();
                }
                catch (Exception e)
                {

                }
            }
        }

        public abstract void ProcessPacket(string data);

    }
}
