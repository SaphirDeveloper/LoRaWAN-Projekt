using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class Rxpk : SemtechPacket
    {
        public string time;
        public long tmms;
        public int tmst;
        public float freq;
        public int chan;
        public int rfch;
        public int stat;
        public string modu;
        public string datr;
        public string codr;
        public int rssi;
        public float lsnr;
        public int size;
        public string data;

        public Rxpk(string time, int tmms, long tmst, float freq, int chan, int rfch, int stat, string modu,
            string datr, string codr, int rssi, float lsnr, int size, string data)
        {
            this.time = time;
            this.tmst = tmms;
            this.tmms = tmst;
            this.freq = freq;
            this.chan = chan;
            this.rfch = rfch;
            this.stat = stat;
            this.modu = modu;
            this.datr = datr;
            this.codr = codr;
            this.rssi = rssi;
            this.lsnr = lsnr;
            this.size = size;
            this.data = data;
        }
    }
}