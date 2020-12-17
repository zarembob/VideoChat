using ChatClient;
using ChatServer.Entity;
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
        private const int port = 2020;
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
            string currentUser = "";
            Client currentClient = new Client();
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
                            Port = client2.Port,
                            address = ""

                        };
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
                        currentUser = dbHelper.GetUserName(c);
                        if (dbHelper.IsLogin(c))
                        {
                            var l = dbHelper.GetFriends(client2.Email);
                            LoginFriendNames.Add("Granted");
                            Console.WriteLine(dbHelper.GetUserName(c));
                            LoginFriendNames.Add(dbHelper.GetUserName(c));
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
                        dbHelper.setIP(ip, currentUser);
                    }
                    else if (res == "GetFriendData")
                    {
                        var serializer = new XmlSerializer(typeof(string));
                        var name = (string)serializer.Deserialize(stream);
                        currentClient = dbHelper.GetClient(name);
                    }
                    else if (res == "SetFriendData")
                    {
                        GetFriendData getFriend = new GetFriendData();
                        getFriend.address = currentClient.address;
                        getFriend.port = currentClient.Port;
                        var serializer = new XmlSerializer(typeof(GetFriendData));
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
                        if (currentFriend != null)
                        {
                            Friend f = new Friend()
                            {
                                Name = currentFriend.Name,
                                Email = currentFriend.Email,
                                ClientEmail = currentClient.Email,
                                Port = 2021,
                                address = (Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]).ToString()
                            };  dbHelper.AddFriend(f);
                            var serializer = new XmlSerializer(typeof(string));
                            serializer.Serialize(stream, "Granted");
                        }

                    }
                }

            }
        }
    }
}

