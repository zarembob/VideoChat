namespace ChatServer
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ClientModel : DbContext
    {
   
        public ClientModel()
            : base("name=ClientModel")
        {
        }
        public DbSet<Client> Clients { get; set; }

    }

   
}