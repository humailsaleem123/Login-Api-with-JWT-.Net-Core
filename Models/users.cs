using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class users
    {

        [Key]
        public int ID { get; set; }

        public string username { get; set; }
   
        public string password { get; set; }

        public DateTime dateCreated { get; set; }
    }
}
