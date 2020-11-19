using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace MessengerBase
{
    public class GeneralMethods
    {
        public static string Serialize(List<string> users)
        {
            var json = JsonConvert.SerializeObject(users);
            json = EncodeTo64(json);
            return json;
        }
        public static Hashtable Deserialize(string serializedJson)
        {
            serializedJson = DecodeFrom64(serializedJson);
            Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(serializedJson);
            return hash;
        }
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
