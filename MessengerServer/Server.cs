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

        static void Main()
        {
            Setup();
            TcpListener serverSocket = new TcpListener(serverIp, serverPort);
            serverSocket.Start();
            Console.WriteLine("Chat Server Started");
            int counter = 0;
            try
            {
                while ((true))
                {
                    bytesRead = 0;
                    counter += 1;
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    byte[] bytesFrom = new byte[10025];
                    string username = null;

                    NetworkStream networkStream = clientSocket.GetStream();
                    bytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    username = encoder.GetString(bytesFrom, 0, bytesRead);
                    try
                    {
                        clientList.Add(username, clientSocket);
                    }
                    catch (ArgumentException ae)
                    {
                        // ToDo: Tell client username is already taken.
                        // Username already taken.
                        UsernameAlreadyTaken();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    // let users know someone is connected
                    CommunicationHandler.SendOnlineUsers();

                    Console.WriteLine(username + " Joined chat room ");
                    HandleClient client = new HandleClient();
                    client.StartClientThread(clientSocket, username);
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

        private static void UsernameAlreadyTaken() { }
        public static void Setup()
        {
            if (ConfigurationManager.GetSection("serverAddressSettings/serverAddress") is NameValueCollection customSettings)
            {
                serverIp = IPAddress.Parse(customSettings["serverIpAdress"].ToString());
                serverPort = Convert.ToInt32(customSettings["serverPort"]);
            }
        }

    }
    public class HandleClient
    {
        TcpClient clientSocket;
        string clNo;
        int bytesRead;
        public void StartClientThread(TcpClient inClientSocket, string clNo)
        {
            clientSocket = inClientSocket;
            clNo = clNo;
            Thread ctThread = new Thread(Communicate);
            ctThread.Start();
        }

        private void Communicate()
        {
            byte[] bytesFrom = new byte[10025];
            int requestCount = 0;
            try
            {
                while ((true))
                {
                    bytesRead = 0;
                    requestCount++;
                    NetworkStream networkStream = clientSocket.GetStream();
                    bytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    string dataFromClient = encoder.GetString(bytesFrom, 0, bytesRead);

                    Package pck = new Package();
                    pck = pck.Deserialize(dataFromClient);

                    switch (pck.PackageType)
                    {
                        // FORWARD MESSAGE
                        case PackageTypeEnum.NewMessage:
                            if (CommunicationHandler.SendMessage(pck.Serialize(), (TcpClient)Server.clientList[pck.ReceiverUser.Username]))
                            {
                                Console.WriteLine(String.Format("Message is forwarded to {0}, coming from {1}.", pck.ReceiverUser.Username, pck.SenderUser.Username));
                            }
                            else
                            {
                                Package fail = new Package
                                {
                                    PackageType = PackageTypeEnum.Fail
                                };
                                CommunicationHandler.SendMessage(fail.Serialize(), (TcpClient)Server.clientList[pck.ReceiverUser.Username]);
                            }
                            break;
                    }
                    dataFromClient = pck.Context;
                    Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    string rCount = Convert.ToString(requestCount);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                CommunicationHandler.UserDisconnected(clientSocket);
                clientSocket.Close();
            }
        }
    }
}