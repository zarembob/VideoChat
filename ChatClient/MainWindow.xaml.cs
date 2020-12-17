using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ClientDTO currentClient;
        private static TcpListener server;
        private static ClientHelper helper;
       
        public MainWindow(ClientDTO _client)
        {

            InitializeComponent();
            _client.Friends.Remove(_client.Username);
            _client.Friends.Remove("Granted");
            this.DataContext = _client;
            currentClient = _client;
            server = new TcpListener(IPAddress.Parse(currentClient.address), currentClient.Port);
            server.Start();
            server.BeginAcceptTcpClient(DoAcceptTcpClientCallback, server);
           // server.Stop();

        }


        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            GetData(client);

        }

        private void GetData(TcpClient _client)
        {
            byte[] bytes = new byte[1024];
            string data = "";
            NetworkStream stream = _client.GetStream();
            int i;
            i = stream.Read(bytes, 0, bytes.Length);
            while (i != 0)
            {

                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                data = data.ToUpper();
                i = stream.Read(bytes, 0, bytes.Length);
            }
            string check = CheckData(data);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(check);
            stream.Write(msg, 0, msg.Length);
            if (check == "true")
            {
                GetFriendData dataF = new GetFriendData();
                helper.Option(data);
                helper.AcceptFriendData(ref dataF);
                this.Content = new Call(IPAddress.Parse(dataF.address),dataF.port,currentClient);
            }
        }

        private string CheckData(string data)
        {
            if (data == "Call")
            foreach (var item in currentClient.Friends)
            {
                if (data == item)
                {
                    
                    return "true";
                }

            }
            return "false";
        }

        private void Phone_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }


        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetFriendData data = new GetFriendData();
            helper.Option(FriendList.SelectedItem.ToString());
            helper.AcceptFriendData(ref data);
            TcpClient client = new TcpClient(data.address.ToString(), data.port);
            string message = currentClient.Username;
            byte[] dataSend = System.Text.Encoding.ASCII.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(dataSend, 0, dataSend.Length);
            dataSend = new byte[256];
            string response = "";
            int bytes = stream.Read(dataSend, 0, dataSend.Length);
            response = System.Text.Encoding.ASCII.GetString(dataSend, 0, bytes);
            if (response == "true")
            {
                //Phone.Content = "true";
                  this.Content = new Call(IPAddress.Parse(data.address),data.port,currentClient);

            }
            stream.Close();
            client.Close();
        }
    }
}
