using MessengerBase.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MessengerBase.Models
{
    public class Package
    {
        public User SenderUser { get; set; }
        public User ReceiverUser { get; set; }
        public PackageTypeEnum PackageType { get; set; }
        public string Context { get; set; }

        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();

        public Package()
        {
            // Set custom serializers as settings to be able to serialize IPAdress.
            jsonSerializerSettings.Converters.Add(new IPAddressConverter());
            jsonSerializerSettings.Converters.Add(new IPEndPointConverter());
            jsonSerializerSettings.Formatting = Formatting.Indented;
        }

        public Package(string message, User senderUser, User receiverUser, PackageTypeEnum packageType)
        {
            Context = message;
            this.SenderUser = senderUser;
            this.ReceiverUser = receiverUser;
            this.PackageType = packageType;

            // Set custom serializers as settings to be able to serialize IPAdress.
            jsonSerializerSettings.Converters.Add(new IPAddressConverter());
            jsonSerializerSettings.Converters.Add(new IPEndPointConverter());
            jsonSerializerSettings.Formatting = Formatting.Indented;
        }

        public void SendPackage(NetworkStream stream)
        {
            string toSend = Serialize();
            try
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] outStream = encoder.GetBytes(toSend);
                stream.Write(outStream, 0, outStream.Length);
                stream.Flush();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error." + e.StackTrace);
            }
        }

        public string Serialize()
        {
            var json = JsonConvert.SerializeObject(this, jsonSerializerSettings);
            json = EncodeTo64(json);
            return json;
        }
        public Package Deserialize(string serializedJson)
        {
            serializedJson = DecodeFrom64(serializedJson);
            Package pck = JsonConvert.DeserializeObject<Package>(serializedJson, jsonSerializerSettings);
            return pck;
        }
        public List<string> DeserializeClients(string serializedJson)
        {
            serializedJson = DecodeFrom64(serializedJson);
            List<string> users = JsonConvert.DeserializeObject<List<string>>(serializedJson, jsonSerializerSettings);
            return users;
        }
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }

    public enum PackageTypeEnum
    {
        Success,
        Fail,
        NewMessage,
        UserConnectedDisconnected
    }
}
