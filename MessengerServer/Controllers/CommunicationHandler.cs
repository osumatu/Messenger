using MessengerBase;
using MessengerBase.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MessengerServer.Controllers
{

    public class CommunicationHandler
    {
        public static bool SendMessage(string message, TcpClient receiverClient)
        {
            try
            {
                NetworkStream broadcastStream = receiverClient.GetStream();
                byte[] broadcastBytes = null;

                broadcastBytes = Encoding.ASCII.GetBytes(message);

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        public static void SendOnlineUsers()
        {
            List<string> users = ((ICollection<string>)Server.clientList.Keys).ToList();
            string context = GeneralMethods.Serialize(users);
            Package package = new Package
            {
                PackageType = PackageTypeEnum.UserConnectedDisconnected,
                Context = context
            };
            context = package.Serialize();
            Broadcast(context);
        }
        public static void Broadcast(string package)
        {
            try
            {
                foreach (DictionaryEntry Item in Server.clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    byte[] broadcastBytes = null;

                    broadcastBytes = Encoding.ASCII.GetBytes(package);

                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                }

            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void UserDisconnected(TcpClient client)
        {
            Hashtable temptable = new Hashtable();
            foreach (DictionaryEntry tcp in Server.clientList)
            {
                if (!tcp.Value.Equals(client))
                    temptable.Add(tcp.Key, tcp.Value);
                else
                    Console.WriteLine(String.Format("User {0} has disconnected.", tcp.Key));
            }
            Server.clientList = temptable;
            SendOnlineUsers();
        }
    }
}
