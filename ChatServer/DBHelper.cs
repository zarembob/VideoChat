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
    }
}
