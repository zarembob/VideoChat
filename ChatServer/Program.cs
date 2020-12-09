﻿using ChatClient;
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

            ip = (Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]);
            server = new TcpListener(ip, port);
            server.Start();
            string res="";

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
                            Password = client2.Password

                        };
                        dbHelper.AddClient(c);
                        Console.WriteLine("Finished Register");
                    }
                }
            }
        }
    }
}
