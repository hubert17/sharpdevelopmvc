using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ASPNETWebApp48.Models
{
    // [DbConfigurationType(typeof(MySqlEFConfiguration))] // Uncomment when using MySQL data provider
    public class MyApp48DbContext : DbContext
    {
        public MyApp48DbContext() : base("MyApp48Db") // name_of_dbconnection_string
        {
        }

        // Map model classes to database tables
        public DbSet<UserAccount> Users { get; set; }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Product> Products { get; set; }
        

    }


}

