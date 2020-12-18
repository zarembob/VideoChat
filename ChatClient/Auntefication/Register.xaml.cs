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
        ClientHelper helper = new ClientHelper();
        private void Button_Click(object sender, RoutedEventArgs e)
        {      
            helper.Option("Register");
            var client = new ClientDTO
            {
                Username = username.Text,
                Email = email.Text,
                Password = password.Password,

            };
            helper.SendClient(client);
            helper.Option("CheckRegister");
            var callbackString = helper.AcceptCallback();
            helper.CheckResult(callbackString);

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
