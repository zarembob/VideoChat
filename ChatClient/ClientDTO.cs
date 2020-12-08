using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    [Serializable]
  public  class ClientDTO
    {
        public int Id { get; set; }
        private string username;
        public string Username { get { return username; } set { username = value; } }
        private string email;
        public string Email { get { return email; } set { email = value; } }
        private string password;
        public string Password { get { return password; } set { password = value; } }
     
    }
}
