using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    public class Stat
    {
        
        public String Time;
        public float Lati;
        public float Long;
        public float Ackr;
        public int Alti;
        public int Rxnb;
        public int Rxok;
        public int Rxfw;
        public int Dwnb;
        public int Txnb;
        




        public Stat(string time, float lati, float @long, int alti, int rxnb, int rxok, int rxfw, float ackr, int dwnb, int txnb)
        {
            this.Time = time;
            this.Lati = lati;
            this.Long = @long;
            this.Rxnb = rxnb;
            this.Rxok = rxok;
            this.Rxfw = rxfw;
            this.Ackr += ackr;
            this.Dwnb = dwnb;
            this.Txnb = txnb;
                
             
        }

        
        /*
        static void Main()
        {
            Stat stat = new Stat("2014-01-12 08:59:28 GMT", 46.24000f, 3.25230f, 145, 2, 2, 2, 100.0f, 2, 2);

            string jsonPayload = JsonConvert.SerializeObject(stat);
            
            Stat statObject = JsonConvert.DeserializeObject<Stat>(jsonPayload);

            Console.WriteLine(jsonPayload);
            Console.WriteLine(stat);
            Console.WriteLine(stat.Time);
            Console.ReadKey();
        }
        */


    }
}
