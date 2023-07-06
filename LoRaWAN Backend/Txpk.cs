using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoRaWAN
{
    internal class Txpk : SemtechPacket
    {
        public bool imme;
        public int tmst;
        public long tmms;
        public float freq;
        public int rfch;
        public int powe;
        public string modu;
        public string datr;
        public string codr;
        public int fdev;
        public bool ipol;
        public int prea;
        public int size;
        public string data;
        public bool ncrc;

        public Txpk(bool imme, int tmst, long tmms, float freq, int rfch, int powe, string modu, 
            string datr, string codr , int fdev, bool ipol, int prea, int size, string data, bool ncrc) 
        { 
            this.imme = imme;
            this.tmst = tmst;
            this.tmms = tmms;
            this.freq = freq;
            this.rfch = rfch;
            this.powe = powe;
            this.modu = modu;
            this.datr = datr;
            this.codr = codr;
            this.fdev = fdev;
            this.ipol = ipol;
            this.size = size;
            this.data = data;
            this.ncrc = ncrc;
        }
    }
}
