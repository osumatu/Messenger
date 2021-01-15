using System.Net;
using System.Net.Sockets;

namespace MessengerBase.Models
{
    public class User
    {
        public string Username { get; set; }
        public IPAddress IpAdress { get; set; }
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
            this.Username = username;
            IpAdress = GetLocalIPAddress();
        }
        public User() { }
    }
}
