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
        public MainWindow(ClientDTO _client)
        {
            
            InitializeComponent();
            _client.Friends.Remove("Granted");
            _client.Friends.Remove(_client.Username);
            if(_client.Friends.Count==0)
            {
                Friends.Items.Add("No friends");
            }
            this.DataContext = _client;
            

        }

        private void Phone_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
