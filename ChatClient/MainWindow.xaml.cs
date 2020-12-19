using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        // private const int port = 2020;
        private ClientDTO currentClient;
        UdpClient server;
        private static ClientHelper helper = new ClientHelper();
        Thread thread;
        public MainWindow()
        {

            InitializeComponent();
            Login l = new Login();
            l.ShowDialog();
   
            Data.client.Friends.Remove(Data.client.Username);
            Data.client.Friends.Remove("Granted");
            currentClient = Data.client;
            try
            {
                server = new UdpClient(currentClient.Port);
            }
          catch(SocketException ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
            this.DataContext = currentClient;        
            thread = new Thread(Receive);
            thread.IsBackground = true;
            try
            {
                thread.Start();
            }
            catch(OutOfMemoryException ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }

            foreach (var item in currentClient.Friends)
            {
                FriendList.Items.Add(item);
            }
        }

        static int port;
        private void Receive()
        {
            while (true)
            {

                IPEndPoint ep = null;
                try
                {
                    var data = server.Receive(ref ep);
                    if (Encoding.UTF8.GetString(data) == "true")
                    {

                        ep = null;

                        List<string> res = new List<string>();
                        for (int i = 0; i < 4; i++)
                        {

                            var r = server.Receive(ref ep);
                            res.Add(Encoding.UTF8.GetString(r));
                        }

                        GetFriendDataDTO dataF = new GetFriendDataDTO();
                        dataF.port = Int32.Parse(res[2]);
                        dataF.address = res[3];
                        GetClientDataDTO dataC = new GetClientDataDTO();
                        dataC.port = Int32.Parse(res[0]);
                        dataC.address = res[1];

                        Dispatcher.Invoke(() =>
                        {
                            this.Content = new Call(dataF, dataC, server);
                        });
                        thread.Abort();

                    }
                }
               catch(SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                    this.Close();
                }
                
            }
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
            #region Bad
            //GetFriendDataDTO data = new GetFriendDataDTO();
            //helper.Option("GetFriendData");
            //helper.Option(FriendList.SelectedItem.ToString());
            //helper.Option("SetFriendData");
            //helper.AcceptFriendData(ref data);
            //TcpClient client = new TcpClient(data.address.ToString(), data.port);
            //string message = currentClient.Username;
            //byte[] dataSend = System.Text.Encoding.ASCII.GetBytes(message);
            //NetworkStream stream = client.GetStream();
            //stream.Write(dataSend, 0, dataSend.Length);
            //dataSend = new byte[256];
            //string response = "";
            //int bytes = stream.Read(dataSend, 0, dataSend.Length);
            //response = System.Text.Encoding.ASCII.GetString(dataSend, 0, bytes);
            //if (response == "true")
            //{
            //    IPAddress address = IPAddress.Parse(data.address);
            //    this.Content = new Call(data, currentClient);

            //}
            //stream.Close();
            //client.Close();
            #endregion

            thread.Abort();
            #region haram
            // GetFriendDataDTO dataF = new GetFriendDataDTO();
            // helper.Option("GetFriendData");
            // helper.Option(FriendList.SelectedItem.ToString());
            // helper.Option("SetFriendData");
            // helper.AcceptFriendData(ref dataF);
            // this.Content = new Call(dataF, currentClient);
            //
            // UdpClient client = new UdpClient();
            // byte[] sendBytes = Encoding.ASCII.GetBytes(currentClient.Username);
            //
            // client.Send(sendBytes, sendBytes.Length, currentClient.address, dataF.port);
            //
            // IPEndPoint ep = new IPEndPoint(IPAddress.Parse(dataF.address), dataF.port);
            // var data = server.Receive(ref ep);
            // MemoryStream byteStream = new MemoryStream(data);
            // string check = Encoding.ASCII.GetString(byteStream.ToArray());
            // client.Close();
            //
            // if (check == "true")
            // {
            //     this.Content = new Call(dataF, currentClient);
            //
            // }
            #endregion
            GetFriendDataDTO dataF = new GetFriendDataDTO();
            helper.Option("GetFriendData");
            helper.Option(FriendList.SelectedItem.ToString());
            helper.Option("SetFriendData");
            helper.AcceptFriendData(ref dataF);

            port = dataF.port;

            GetClientDataDTO dataC = new GetClientDataDTO();
            helper.Option("SetClientData");
            helper.AcceptClientData(ref dataC);

            List<string> call = new List<string>();
            call.Add(dataF.port.ToString());
            call.Add(dataF.address);
            call.Add(dataC.port.ToString());
            call.Add(dataC.address);
        
          
            UdpClient udpClient = new UdpClient();
            string tmp = "true";
            udpClient.Send(Encoding.UTF8.GetBytes(tmp), tmp.Length, dataF.address, dataF.port);            
            for (int i = 0; i < call.Count; i++)
            {
                Thread.Sleep(100);
                udpClient.Send(Encoding.UTF8.GetBytes(call[i]), call[i].Length, dataF.address, dataF.port);

            }
            Dispatcher.Invoke(() => { 
            this.Content = new Call(dataF, dataC, server);
            });
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            helper.Option("Add");
            helper.Option(AddFriend.Text);
            helper.Option("GetFriend");
            string re = helper.AcceptCallback();
            if (re == "Granted")
            {

                currentClient.Friends.Add(AddFriend.Text);

                FriendList.Items.Add(AddFriend.Text);

                AddFriend.Text = "Yes";
            }
            else
            {
                AddFriend.Text = "haram";
            }
        }
    }
}
