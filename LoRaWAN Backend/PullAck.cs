﻿namespace LoRaWAN
{
    class PullAck : SemtechPacket
    {
        public PullAck(string token)
        {

            this.ProtocolVersion = "2";
            this.Token = token;
            this.Id = "0x04";
        }

        public override string ToString()
        {
            // Include the specific attributes of the PushAck class in the string representation
            return $"PushAck [ProtocolVersion: {ProtocolVersion}, Token: {Token}, Id: {Id}]";
        }
    }
}
