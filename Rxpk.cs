using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class Rxpk : SemtechPacket
    {
        public string Time;
        public long Tmms;
        public int Tmst;
        public float Freq;
        public int Chan;
        public int Rfch;
        public int Stat;
        public string Modu;
        public string Datr;
        public string Codr;
        public int Rssi;
        public float Lsnr;
        public int Size;
        public string Data;

        public Rxpk(string time, int tmms, long tmst, float freq, int chan, int rfch, int stat, string modu,
            string datr, string codr, int rssi, float lsnr, int size, string data)
        {
            this.Time = time;
            this.Tmst = tmms;
            this.Tmms = tmst;
            this.Freq = freq;
            this.Chan = chan;
            this.Rfch = rfch;
            this.Stat = stat;
            this.Modu = modu;
            this.Datr = datr;
            this.Codr = codr;
            this.Rssi = rssi;
            this.Lsnr = lsnr;
            this.Size = size;
            this.Data = data;
        }
    }
}