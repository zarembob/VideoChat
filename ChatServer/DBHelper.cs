using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class DBHelper
    {
        private ClientModel Context;
        public DBHelper()
        {
            Context = new ClientModel();
        }
        public void AddClient(Client client)
        {
            Context.Clients.Add(client);
            Context.SaveChanges();
        }
        public void AddFriend(Friend f)
        {
            Context.Friends.Add(f);
            Context.SaveChanges();
        }
        public List<Friend> GetFriends(string Email)
        {
            var list = from x in Context.Friends where x.ClientEmail == Email select x;
            return list.ToList();
        }
        public string GetUserName(Client c)
        {
            var name = from x in Context.Clients where x.Email == c.Email select x.Name;
            return name.ToList().FirstOrDefault();
        }
        public bool IsLogin(Client c)
        {
            bool flag = false;
            foreach (Client item in Context.Clients)
            {
                if (item.Email == c.Email && item.Password == c.Password)
                {

                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public bool IsRegister(Client c)
        {
            bool flag = false;
            foreach (Client item in Context.Clients)
            {
                if (item.Name == c.Name || item.Email == c.Email)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
    }
}
