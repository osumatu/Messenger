using MessengerBase.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MessengerClient
{
    static class Program
    {
        public static User user = new User();
        public static IPAddress serverIp = null;
        public static int serverPort = -1;
        public static TcpClient clientSocket = new TcpClient();
        public static NetworkStream serverStream = default(NetworkStream);
        public static List<Thread> activeThreads = new List<Thread>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            NameValueCollection customSettings = ConfigurationManager.GetSection("serverAddressSettings/serverAddress") as System.Collections.Specialized.NameValueCollection;
            if (customSettings != null)
            {
                serverIp = IPAddress.Parse(customSettings["serverIpAdress"].ToString());
                serverPort = Convert.ToInt32(customSettings["serverPort"]);

            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Views.Login());

        }
    }
}
