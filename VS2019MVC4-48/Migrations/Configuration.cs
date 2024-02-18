namespace ASPNETWebApp48.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ASPNETWebApp48.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<MyApp48DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MyApp48DbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Products.AddOrUpdate(
              p => p.Name,
                // Procter & Gamble products
                new Product { Name = "Tide Laundry Detergent", UnitPrice = 350.00m, Description = "Original scent, 64 loads", Unit = "bottle" },
                new Product { Name = "Pampers Diapers", UnitPrice = 999.00m, Description = "Size 4, 120 count", Unit = "box" },
                new Product { Name = "Gillette Fusion Razor", UnitPrice = 199.75m, Description = "With 1 blade cartridge", Unit = "pack" },

                // Unilever products
                new Product { Name = "Dove Beauty Bar", UnitPrice = 75.00m, Description = "4 bars, moisturizing cream", Unit = "pack" },
                new Product { Name = "Hellmann's Mayonnaise", UnitPrice = 120.00m, Description = "Real, 30 oz", Unit = "jar" },
                new Product { Name = "Axe Body Spray", UnitPrice = 190.00m, Description = "Phoenix scent, 4 oz", Unit = "bottle" },

                // Nestle products
                new Product { Name = "Nescafe Instant Coffee", UnitPrice = 170.00m, Description = "Classic, 12 oz", Unit = "jar" },
                new Product { Name = "KitKat Chocolate Bar", UnitPrice = 30.00m, Description = "Milk chocolate, 1.5 oz", Unit = "bar" },
                new Product { Name = "Nestle Pure Life Water", UnitPrice = 180.00m, Description = "24-pack, 16.9 oz bottles", Unit = "pack" },

                // Coca-Cola products
                new Product { Name = "Coca-Cola 1 Liter", UnitPrice = 30.00m, Description = "1 Liter bottle of Coca-Cola", Unit = "bottle" },
                new Product { Name = "Sprite 12 oz", UnitPrice = 12.00m, Description = "12 fl oz can of Sprite", Unit = "can" },
                new Product { Name = "Minute Maid Pulpy Orange", UnitPrice = 25.00m, Description = "Orange juice with pulp, 16.9 fl oz bottle", Unit = "bottle" },

                // Alcoholic beverages
                new Product { Name = "Tanduay White Rum", UnitPrice = 125.375m, Description = "750ml", Unit = "bottle" },
                new Product { Name = "Red Horse Beer", UnitPrice = 118.0625m, Description = "1000ml", Unit = "bottle" },
                new Product { Name = "Emperador Light", UnitPrice = 165.442m, Description = "1L blended premium brandy", Unit = "bottle" }
            );
        }
    }
}
