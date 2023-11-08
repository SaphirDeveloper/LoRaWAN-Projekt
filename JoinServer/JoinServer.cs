﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoRaWAN;
using LoRaWAN.BackendPackets;
using LoRaWAN.PHYPayload;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoinServer
{
    public class JoinServer : Server
    {
        private HttpClient _httpClient = new HttpClient();

        public JoinServer() : base(Appsettings.JoinServerURL) { }

        public override void ProcessPacket(string json)
        {
            Console.WriteLine(json);
            JObject jObject = JObject.Parse(json);
            if ((bool)(jObject["MessageType"]?.Value<string>().Equals("JoinReq")))
            {
                PHYpayload phyPayload = PHYpayloadFactory.CreatePHYpayloadJoinAccept("E8B0C9", "000000", "00000000", "94", "35", "73373778");
                JoinAns joinAns = new JoinAns();
                joinAns.MessageType = "JoinAns";
                joinAns.PhyPayload = phyPayload.Hex;
                _httpClient.PostAsJsonAsync(Appsettings.NetworkServerURL, JsonConvert.SerializeObject(joinAns)).Wait();
            }
            else
            {
                Console.WriteLine("Cannot process JSON...");
            }
        }
    }
}
