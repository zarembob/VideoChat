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
        ClientHelper helper = new ClientHelper();
        private const int port = 2020;
        public Login()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            helper.Option("Login");
            var client = new ClientDTO
            {
                //Username = username.Text,
                Email = email.Text,
                Password = password.Password,
            };

            helper.SendClient(client);
            helper.Option("Check");
            client.Friends = helper.AcceptCallbackLogin();
           bool check= helper.CheckResultLogin(ref client);
            if(check)
            {
                Data.client = client;
                this.Close();
            }
            else
            {
              
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Content = new Register();
        }
    }
}
