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
        public Client GetUserName(Client c)
        {
            foreach (Client item in Context.Clients)
            {
                if (item.Email == c.Email)
                {
                    return item;
                }
            }
            return null;
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

        public Client GetClient(string name)
        {
            foreach(Client c in Context.Clients)
            {
                if(c.Name==name)
                {
                    return c;
                }
            }
            return null;
        }
        

        public void setIP(string ip,string current)
        {
            for (int i = 0; i < Context.Clients.Count(); i++)
            {
                if((Context.Clients.ToList())[i].Name==current)
                {
                    (Context.Clients.ToList())[i].address = ip;
                }
            }
            Context.SaveChanges();
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

        public int GetLastPort()
        {
            int port = 0;
            foreach (Client item in Context.Clients)
            {
                port = item.Port;
            }
            return port;
        }
        public bool CheckAddFriend(string currentName)
        {
            bool flag = false;
            foreach (Friend item in Context.Friends)
            {
                if (item.Name == currentName )
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
    }
}
