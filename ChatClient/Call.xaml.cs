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

        int myPort = 0;

        GetFriendDataDTO friend;
        UdpClient server;

        public Call(GetFriendDataDTO data, GetClientDataDTO client, UdpClient _server)
        {
            Dispatcher.Invoke(() => { 
            InitializeComponent();
            this.DataContext = this;
            friend = data;
            myPort = client.port;
            server = _server;
            GetVideoDevices();
            });
            var thread = new Thread(Receive);
            thread.IsBackground = true;
            thread.Start();
            // this.Closing += MainWindow_Closing;
        }
        #region VideoCall
        private void Receive()
        {
            while (true)
            {
                IPEndPoint ep = null;

                var data = server.Receive(ref ep);
                Dispatcher.Invoke(() =>
                {
                    MemoryStream byteStream = new MemoryStream(data);
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = byteStream;
                    image.EndInit();
                    videoFriend.Source = image;


                });
            }
        }

        public byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        #endregion
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
                BitmapImage friendBi = new BitmapImage();
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bi = bitmap.ToBitmapImage();
                }

                Dispatcher.Invoke(() =>
                {

                    UdpClient client = new UdpClient();
                    byte[] sendBytes = new byte[1024];
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                    Thread.Sleep(10);
                    sendBytes = ImageSourceToBytes(encoder, bi);
                    client.Send(sendBytes, sendBytes.Length, friend.address, friend.port);
                });

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi; }));

            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
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
