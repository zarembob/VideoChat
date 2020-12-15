using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Client
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]      
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public ICollection<Friend> friends { get; set; }
        public Client()
        {
            friends = new List<Friend>();
        }
    }
    public class Friend
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public int? ClientId { get; set; }
    }
}
