using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    [Serializable]

    public class ClientDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
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
        private string email;
        public string Email { get { return email; } set { email = value; } }
        private string password;
        public string Password { get { return password; } set { password = value; } }
        public List<string> friends { get; set; }

    }
}
