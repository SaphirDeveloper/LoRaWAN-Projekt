using System.Net;
using System.Text;

namespace Unit_Test.Helper
{
    internal class FakeServer
    {
        private Thread _serverThread;
        private HttpListener _listner;
        public Queue<string> ReceivedRequests { get; private set; }

        public FakeServer(string url)
        {
            if (!url.EndsWith('/')) url += "/";

            _listner = new HttpListener();
            _listner.Prefixes.Add(url);
            _serverThread = new Thread(AwaitData);
            ReceivedRequests = new Queue<string>();
        }

        public void AwaitData()
        {
            while (_listner.IsListening)
            {
                try
                {
                    // Await connection
                    HttpListenerContext context = _listner.GetContext();

                    // Read Request
                    HttpListenerRequest request = context.Request;
                    if (request.HasEntityBody)
                    {
                        Stream body = request.InputStream;
                        Encoding encoding = request.ContentEncoding;
                        StreamReader reader = new StreamReader(body, encoding);

                        ReceivedRequests.Enqueue(reader.ReadToEnd());
                    }

                    // Send Response
                    HttpListenerResponse response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
                catch (Exception)
                {
                    // Ignore Exception
                }
            }
        }

        public void Start()
        {
            _listner.Start();
            _serverThread.Start();
        }

        public void Stop()
        {
            _listner.Stop();
            _serverThread.Join();
        }
    }
}
