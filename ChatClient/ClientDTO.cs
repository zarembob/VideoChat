using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChatClient
{
    [Serializable]

    public class ClientDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public int Id { get; set; }
        private string username;
        public string Username { get { return username; } set { username = value; OnPropertyChanged("username"); } }
        public string Email { get; set; }
        public string Password { get; set; }
        private List<string> friends = new List<string>();
        public List<string> Friends
        {
            get { return friends; }
            set
            {
                friends = value;
                OnPropertyChanged("friends");
            }
        }
        public int Port { get; set; }

        public string address { get; set; }
    }
    public class FriendDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Port { get; set; }
        public string IpAddress { get; set; }
    }
}
