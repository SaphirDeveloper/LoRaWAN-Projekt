namespace LoRaWAN
{
    public abstract class Server
    {
        // Field
        private WebApplication _webApplication;


        // Constructor
        public Server(string url)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls(new[] {url});

            _webApplication = builder.Build();
            _webApplication.MapPost("", async (HttpRequest request) =>
            {
                StreamReader reader = new StreamReader(request.Body);
                string json = await reader.ReadToEndAsync();
                Console.WriteLine(request.Headers.ContentType);
                ProcessPacket(json);
            });
        }

        public void Start()
        {
            _webApplication.StartAsync();
        }

        public void WaitForShutdown()
        {
            _webApplication.WaitForShutdown();
        }

        public void Shutdown()
        {
            _webApplication.StopAsync();
        }

        public abstract void ProcessPacket(string json);

    }
}
