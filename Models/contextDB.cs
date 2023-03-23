using Microsoft.EntityFrameworkCore;

namespace Login.Models
{
    public class contextDB : DbContext
    {

        public contextDB(DbContextOptions<contextDB> options) : base(options) {
        
        }
      public DbSet<users> tbl_register { get; set; }

        public DbSet<products> tbl_products { get; set; }


    }
}
