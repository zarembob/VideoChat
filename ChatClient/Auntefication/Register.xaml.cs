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
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : UserControl
    {
        private const int port = 2020;

        public Register()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
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
            Option("CheckRegister");
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
            if (callbackString == "Done")
            {
                MainWindow main = new MainWindow();
                main.Show();
                var tmp = this.Parent;
                (tmp as Window).Close();

            }
            else if (callbackString == "Error")
            {
                // bruh.Content = "Denied";

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Login l = new Login();
            l.Show();
            var parent = this.Parent;
            (parent as Window).Close();
        }
    }
}
