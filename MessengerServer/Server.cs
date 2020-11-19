using MessengerBase.Models;
using MessengerServer.Controllers;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessengerServer
{
    public class Server
    {
        public static IPAddress serverIp = null;
        public static int serverPort = -1;
        public static Hashtable clientList = new Hashtable();
        static int bytesRead;

        static void Main(string[] args)
        {
            connect();
            TcpListener serverSocket = new TcpListener(serverIp, serverPort);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Chat Server Started");
            counter = 0;
            try
            {
                while ((true))
                {
                    bytesRead = 0;
                    counter += 1;
                    clientSocket = serverSocket.AcceptTcpClient();
                    byte[] bytesFrom = new byte[10025];
                    string dataFromClient = null;

                    NetworkStream networkStream = clientSocket.GetStream();
                    bytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    dataFromClient = encoder.GetString(bytesFrom, 0, bytesRead);
                    try
                    {
                        clientList.Add(dataFromClient, clientSocket);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);//user already logged in
                    }
                    // let users know someone is connected
                    CommunicationHandler.sendOnlineUsers();

                    Console.WriteLine(dataFromClient + " Joined chat room ");
                    handleClient client = new handleClient();
                    client.startClient(clientSocket, dataFromClient);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            /*
              clientSocket.Close();
              serverSocket.Stop();
              Console.WriteLine("exit");
              Console.ReadLine();*/
        }

        public static void connect()
        {
            NameValueCollection customSettings = ConfigurationManager.GetSection("serverAddressSettings/serverAddress") as System.Collections.Specialized.NameValueCollection;
            if (customSettings != null)
            {
                serverIp = IPAddress.Parse(customSettings["serverIpAdress"].ToString());
                serverPort = Convert.ToInt32(customSettings["serverPort"]);
            }
        }

    }
    public class handleClient
    {
        TcpClient clientSocket;
        string clNo;
        int bytesRead;
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            clientSocket = inClientSocket;
            clNo = clineNo;
            Thread ctThread = new Thread(communicate);
            ctThread.Start();
        }

        private void communicate()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            string rCount = null;
            requestCount = 0;
            try
            {
                while ((true))
                {

                    bytesRead = 0;
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    bytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    dataFromClient = encoder.GetString(bytesFrom, 0, bytesRead);

                    Package pck = new Package();
                    pck = pck.Deserialize(dataFromClient);

                    switch (pck.packageType)
                    {
                        // FORWARD MESSAGE
                        case 1:
                            if (CommunicationHandler.sendMessage(pck.Serialize(), pck.receiverUser.username))
                            {
                                Console.WriteLine(String.Format("Message is forwarded to {0}, coming from {1}.", pck.receiverUser.username, pck.senderUser.username));
                            }
                            else
                            {
                                Package fail = new Package();
                                fail.receiverOffline();
                                CommunicationHandler.sendMessage(fail.Serialize(), pck.senderUser.username);
                            }
                            break;
                    }
                    dataFromClient = pck.context;
                    Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                CommunicationHandler.userDisconnected(clientSocket);
                clientSocket.Close();
            }
        }
    }
}