using System.Text;
using LoRaWAN.SemtechProtocol.Data;
using Newtonsoft.Json;



// Send a HTTP POST Request with JSON every 3 seconds
string networkServerURL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["NetworkServerURL"];
while (true)
{
    Thread.Sleep(3000);
    Stat stat = new Stat(DateTime.Now.ToString(), 46.24000f, 3.25230f, 145, 2, 2, 2, 100.0f, 2, 2);
    StringContent content = new StringContent(JsonConvert.SerializeObject(stat), Encoding.UTF8, "application/json");

    HttpClient client = new HttpClient();

    try
    {
        HttpResponseMessage response = client.PostAsync(networkServerURL, content).Result;
        Console.WriteLine(response.ToString());
    }
    catch(Exception e)
    {
        Console.WriteLine(e.Message);
    }
    Console.WriteLine();
}
