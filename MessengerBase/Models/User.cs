using System.Net;
using System.Net.Sockets;

namespace MessengerBase.Models
{
    public class User
    {
        public string username { get; set; }
        public IPAddress ipAdress { get; set; }
        private IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }
        public User(string username)
        {
            this.username = username;
            ipAdress = GetLocalIPAddress();
        }
        public User() { }
    }
}
