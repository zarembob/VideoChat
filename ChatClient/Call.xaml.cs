using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
using System.Net.Sockets;
using System.IO;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for Call.xaml
    /// </summary>
    public partial class Call : UserControl, INotifyPropertyChanged
    {
        #region Public properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; this.OnPropertyChanged("CurrentDevice"); }
        }
        private FilterInfo _currentDevice;

        #endregion


        #region Private fields

        private IVideoSource _videoSource;

        #endregion
        private IPAddress adress;
        private int port;
      //  UdpClient udpClient;
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
ProtocolType.Udp);
        IPEndPoint p;
        byte[] sendBytes;
        byte[] receiveBytes;
        public Call(IPAddress address, int port, ClientDTO client)
        {
            InitializeComponent();
            p = new IPEndPoint(address, port);

         //   udpClient = new UdpClient();
            adress = address;
            this.port = port;
            GetVideoDevices();

        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCamera();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartCamera();
        }

        private void video_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bi;
                BitmapImage friendBi=new BitmapImage();
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bi = bitmap.ToBitmapImage();
                }
                
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    sendBytes = ms.ToArray();
                }
                try
                {
                    sock.BeginConnect(p, ConnectionCallback, sock);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                using (var ms = new System.IO.MemoryStream(receiveBytes))
                {

                    friendBi.BeginInit();
                    friendBi.CacheOption = BitmapCacheOption.OnLoad; // here
                    friendBi.StreamSource = ms;
                    friendBi.EndInit();
                    
                }
                //  udpClient.BeginSend(sendBytes, sendBytes.Length, adress.ToString(), port, Callback, udpClient);
                bi.Freeze(); 
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi; }));
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoFriend.Source = bi; }));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as Socket;
            client.EndConnect(ar);
            client.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, SendCallback, client);
            client.BeginReceive(receiveBytes, 0, receiveBytes.Length, SocketFlags.None, ReceiveCallback, client);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            receiveBytes = (byte[])ar.AsyncState;
        }

        private void SendCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as Socket;
          client.EndSend(ar);
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
        }

        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(filterInfo);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartCamera()
        {
            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
