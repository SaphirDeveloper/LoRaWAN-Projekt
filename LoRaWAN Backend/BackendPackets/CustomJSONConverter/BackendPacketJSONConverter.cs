using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoRaWAN.BackendPackets.CustomJSONConverter
{
    public class BackendPacketJSONConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BackendPacket).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            var messageType = jo["MessageType"]?.ToString();
            if (string.IsNullOrEmpty(messageType))
            {
                throw new Exception("Missing message type (BackendPacketJSONConverter)");
            }

            BackendPacket packet;
            switch (messageType.ToLower())
            {
                case "joinreq":
                    packet = new JoinReq();
                    break;
                case "joinans":
                    packet = new JoinAns();
                    break;
                case "appskeyans":
                    packet = new AppSKeyAns();
                    break;
                case "appskeyreq":
                    packet = new AppSKeyReq();
                    break;
                case "dataup_conf":
                case "dataup_unconf":
                    packet = new DataUp();
                    break;
                case "datadown_conf":
                case "datadown_unconf":
                    packet = new DataDown();
                    break;
                case "errornotif":
                    packet = new ErrorNotif();
                    break;
                default:
                    packet = new BackendPacket();
                    break;
            }
            serializer.Populate(jo.CreateReader(), packet);

            return packet;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
