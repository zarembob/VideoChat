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
        public bool IsLogin(Client c)
        {
            bool flag = false;
            foreach (Client item in Context.Clients)
            {
                if (item.Name == c.Name && item.Password == c.Password)
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
                if (item.Name == c.Name||item.Email==c.Email)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
    }
}
