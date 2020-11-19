using MessengerBase;
using MessengerBase.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MessengerServer.Controllers
{

    public class CommunicationHandler
    {
        public static bool sendMessage(string msg, string receiverName)
        {
            try
            {
                TcpClient broadcastSocket = (TcpClient)Server.clientList[receiverName];
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                broadcastBytes = Encoding.ASCII.GetBytes(msg);

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
        public static void sendOnlineUsers()
        {
            List<string> users = new List<string>();
            foreach (DictionaryEntry entry in Server.clientList)
            {
                users.Add(entry.Key.ToString());
            }
            string context = GeneralMethods.Serialize(users);
            Package pck = new Package();
            pck.connectionChanged();
            pck.context = context;
            context = pck.Serialize();
            broadcast(context, "server");
        }
        public static void broadcast(string msg, string senderUsername)
        {
            try
            {
                foreach (DictionaryEntry Item in Server.clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    broadcastBytes = Encoding.ASCII.GetBytes(msg);

                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                }

            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
                userDisconnected(senderUsername);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
                userDisconnected(senderUsername);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                userDisconnected(senderUsername);
            }
        }
        public static void userDisconnected(TcpClient client)
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
            sendOnlineUsers();
        }
        public static void userDisconnected(string uname)
        {
            Hashtable temptable = new Hashtable();
            foreach (DictionaryEntry tcp in Server.clientList)
            {
                if (!tcp.Key.Equals(uname))
                    temptable.Add(tcp.Key, tcp.Value);
                else
                    Console.WriteLine(String.Format("User {0} has disconnected.", tcp.Key));
            }
            Server.clientList = temptable;
            sendOnlineUsers();
        }
    }
}
