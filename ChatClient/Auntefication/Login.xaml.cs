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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private const int port = 2020;
        public Login()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Option("Login");
            var client = new ClientDTO
            {
                //Username = username.Text,
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
            Option("Check");
            var callbackString = AcceptCallback();
            CheckResult(callbackString);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Content = new Register();
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
            var client = new TcpClient(Dns.GetHostName(), port);

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
                
                MainWindow main = new MainWindow();
                main.Show();
            }
            else if (callbackString == "Denied")
            {
                check.Content = "Invalid data. Try 1 mo time.";

            }
        }

    }
}
