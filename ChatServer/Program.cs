using ChatClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChatServer
{
    class Program
    {
        private static int port = 2020;

        private static IPAddress ip;
        private static TcpListener server;

        static void Main(string[] args)
        {
            StartService();
            Console.Title = "Server" + server.Server.LocalEndPoint;
        }

        private static void StartService()
        {

            var dbHelper = new DBHelper();
            List<string> LoginFriendNames = new List<string>();
            ip = (Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]);
            string res = "";
            server = new TcpListener(ip, port);
            server.Start();
            bool isLogin = false;
            bool isRegister = false;
            int currentPort = 0;
            int ClientPort = dbHelper.GetLastPort();
            ClientPort++;
            Client currentClient1 = new Client();
            Client currentClient2 = new Client();
            Client currentFriend = new Client();
            while (true)
            {
                Console.WriteLine("Waiting for connecting...");
                var client = server.AcceptTcpClient();
                Console.WriteLine("Connected");

                using (var stream = client.GetStream())
                {
                    var serializer1 = new XmlSerializer(typeof(string));
                    res = (string)serializer1.Deserialize(stream);
                    Console.WriteLine(res);
                }
                client = server.AcceptTcpClient();

                using (var stream = client.GetStream())
                {
                    if (res == "Register")
                    {
                        var serializer2 = new XmlSerializer(typeof(ClientDTO));
                        var client2 = (ClientDTO)serializer2.Deserialize(stream);

                        Client c = new Client
                        {
                            Email = client2.Email,
                            Name = client2.Username,
                            Password = client2.Password,
                            Port = ClientPort,
                            address = ""

                        };
                        ClientPort++;
                        if (!dbHelper.IsRegister(c))
                        {
                            isRegister = true;
                            dbHelper.AddClient(c);
                            Console.WriteLine("Finished Register");
                        }
                        else
                        {
                            isRegister = false;
                        }
                    }
                    else if (res == "Login")
                    {
                        var serializer2 = new XmlSerializer(typeof(ClientDTO));
                        //Error 
                        var client2 = (ClientDTO)serializer2.Deserialize(stream);
                        Client c = new Client
                        {
                            Email = client2.Email,
                            //Name = client2.Username,
                            Password = client2.Password,
                            friends = dbHelper.GetFriends(client2.Email),
                            Port = client2.Port

                        };
                        currentPort = c.Port;
                        currentClient1 = dbHelper.GetUserName(c);
                        if (dbHelper.IsLogin(c))
                        {
                            LoginFriendNames.Clear();
                            var l = dbHelper.GetFriends(currentClient1.Email);
                            LoginFriendNames.Add("Granted");
                            Console.WriteLine(dbHelper.GetUserName(c).Name);
                            LoginFriendNames.Add(dbHelper.GetUserName(c).Name);
                            foreach (var item in l.ToList())
                            {
                                LoginFriendNames.Add(item.Name);
                            }
                            isLogin = true;
                            Console.WriteLine();
                        }
                        else
                        {
                            isLogin = false;
                        }
                    }
                    else if (res == "Check")
                    {
                        var serializer = new XmlSerializer(typeof(List<string>));
                        if (isLogin)
                        {
                            serializer.Serialize(stream, LoginFriendNames);
                            Console.WriteLine("Granted");
                        }
                        // else
                        // {
                        //     serializer.Serialize(stream, "Denied");
                        //     Console.WriteLine("Denied");
                        //
                        // }
                    }
                    else if (res == "CheckRegister")
                    {
                        var serializer = new XmlSerializer(typeof(string));
                        if (isRegister)
                        {
                            serializer.Serialize(stream, "Done");
                            Console.WriteLine("Done");
                        }
                        else
                        {
                            serializer.Serialize(stream, "Error");
                            Console.WriteLine("Error");

                        }
                    }
                    else if (res == "Port")
                    {
                        var serializer = new XmlSerializer(typeof(int));
                        serializer.Serialize(stream, currentPort);

                    }
                    else if (res == "Ip")
                    {
                        var serializer = new XmlSerializer(typeof(string));
                        var ip = (string)serializer.Deserialize(stream);
                        dbHelper.setIP(ip, currentClient1.Name);
                    }
                    else if (res == "GetFriendData")
                    {
                        var serializer = new XmlSerializer(typeof(string));
                        var name = (string)serializer.Deserialize(stream);
                        currentClient2 = dbHelper.GetClient(name);
                    }
                    else if (res == "SetFriendData")
                    {
                        GetFriendDataDTO getFriend = new GetFriendDataDTO();
                        getFriend.address = currentClient2.address;
                        getFriend.port = currentClient2.Port;
                        var serializer = new XmlSerializer(typeof(GetFriendDataDTO));
                        serializer.Serialize(stream, getFriend);
                    }
                    else if (res == "SetClientData")
                    {
                        GetClientDataDTO getFriend = new GetClientDataDTO();
                        getFriend.address = currentClient1.address;
                        getFriend.port = currentClient1.Port;
                        var serializer = new XmlSerializer(typeof(GetClientDataDTO));
                        serializer.Serialize(stream, getFriend);
                    }
                    else if (res == "Add")
                    {

                        var serializer = new XmlSerializer(typeof(string));
                        var friend = (string)serializer.Deserialize(stream);
                        currentFriend = dbHelper.GetClient(friend);

                    }
                    else if (res == "GetFriend")
                    {
                        var serializer = new XmlSerializer(typeof(string));
                        if (currentFriend != null && currentClient1 != currentFriend)
                        {
                            string ipAddress="";
                            var host = Dns.GetHostEntry(Dns.GetHostName());
                            foreach (var ip in host.AddressList)
                            {
                                if (ip.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    ipAddress = ip.ToString();
                                }
                            }
                            if (!dbHelper.CheckAddFriend(currentFriend.Name))
                            {

                                Friend f = new Friend()
                                {
                                    Name = currentFriend.Name,
                                    Email = currentFriend.Email,
                                    ClientEmail = currentClient1.Email,
                                    Port = currentFriend.Port,
                                    address = ipAddress


                                };
                                dbHelper.AddFriend(f);

                                serializer.Serialize(stream, "Granted");
                            }
                            else
                            {
                                serializer.Serialize(stream, "Denied");
                            }
                        }
                        else
                        {
                            serializer.Serialize(stream, "Denied");
                        }
                    }
                }

            }
        }
    }
}

