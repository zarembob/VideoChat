using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChatClient
{
    public class ClientHelper : INotifyPropertyChanged
    {
        private const int port = 2020;
        private string check;
        public string Check
        {
            get { return check; }
            set
            {
                check = value;
                OnPropertyChanged("check");
            }
        }

        public void CheckResult(string callbackString)
        {
            if (callbackString == "Granted")
            {

                MainWindow main = new MainWindow();
                main.Show();
            }
            else if (callbackString == "Denied")
            {
                Check = "Invalid data.";

            }
        }
        
        public string AcceptCallback()
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
        public List<string> AcceptCallbackLogin()
        {

        }
        public void Option(string str)
        {
            var client = new TcpClient(Dns.GetHostName(), port);
            using (var stream = client.GetStream())
            {
                var serializer1 = new XmlSerializer(typeof(string));
                serializer1.Serialize(stream, str);
            }
            client.Close();
        }
        public void SendClient(ClientDTO client)
        {
            var TCP = new TcpClient(Dns.GetHostName(), port);
            using (var stream = TCP.GetStream())
            {
                var serializer = new XmlSerializer(client.GetType());
                serializer.Serialize(stream, client);
            }
            TCP.Close();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

