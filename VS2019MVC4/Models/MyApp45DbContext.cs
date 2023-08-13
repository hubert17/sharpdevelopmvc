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
        public MyApp45DbContext() : base("MyApp45Db") // name_of_dbconnection_string
        {
        }

        // Map model classes to database tables
        public DbSet<UserAccount> Users { get; set; }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Product> Products { get; set; }
        

    }


}

