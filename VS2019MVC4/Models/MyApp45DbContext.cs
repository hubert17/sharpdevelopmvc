using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ASPNETWebApp45.Models
{
    // [DbConfigurationType(typeof(MySqlEFConfiguration))] // Uncomment when using MySQL data provider
    public class MyApp45DbContext : DbContext
    {
        public MyApp45DbContext() : base("PosAccessDb") // name_of_dbconnection_string
        {
        }

        // Map model classes to database tables
        public DbSet<UserAccount> Users { get; set; }

        public DbSet<Song> Songs { get; set; }

        // POS
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
    }


}

