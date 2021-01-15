using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace MessengerBase.Controllers
{
    class IPEndPointConverter : JsonConverter
    {
        private const string Port = "Port";
        private const string Adress = "Address";

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPEndPoint ep = (IPEndPoint)value;
            JObject jo = new JObject
            {
                { Adress, JToken.FromObject(ep.Address, serializer) },
                { Port, ep.Port }
            };
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo[Adress].ToObject<IPAddress>(serializer);
            int port = (int)jo[Port];
            return new IPEndPoint(address, port);
        }
    }
}
