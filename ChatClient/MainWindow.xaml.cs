using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private const int port = 2020;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Option("Login");
            var client = new ClientDTO
            {
                Username = username.Text,
                Email = email.Text,
                Password = password.Password,
            };
            var TCP = new TcpClient(Dns.GetHostName(), port);
            using (var stream = TCP.GetStream())
            {
                var serializer = new XmlSerializer(client.GetType());
                serializer.Serialize(stream, client);
            }
            TCP.Close();
            var callbackString = AcceptCallback();
            CheckResult(callbackString);

        }




        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Option("Register");
            var client = new ClientDTO
            {
                Username = username.Text,
                Email = email.Text,
                Password = password.Password,

            };
            var TCP = new TcpClient(Dns.GetHostName(), port);

            using (var stream = TCP.GetStream())
            {
                var serializer = new XmlSerializer(client.GetType());
                serializer.Serialize(stream, client);
            }
            TCP.Close();
            var callbackString = AcceptCallback();
            CheckResult(callbackString);
        }
        private void Option(string str)
        {
            var client = new TcpClient(Dns.GetHostName(), port);
            using (var stream = client.GetStream())
            {
                var serializer1 = new XmlSerializer(typeof(string));
                serializer1.Serialize(stream, str);
            }
            client.Close();
        }
        private string AcceptCallback()
        {
            string response;
            IPAddress ip = (Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]);
            var server = new TcpListener(ip, port);
            server.Start();
            var client = server.AcceptTcpClient();

            using (var stream = client.GetStream())
            {
                var serializer1 = new XmlSerializer(typeof(string));
                response = (string)serializer1.Deserialize(stream);

            }
            return response;

        }

        private void CheckResult(string callbackString)
        {
            if (callbackString == "Granted")
            {
                bruh.Content = "Granted";
            }
            else if (callbackString == "Denied")
            {
                bruh.Content = "Denied";

            }
        }
    }
}
