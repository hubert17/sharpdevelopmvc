using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNETWebApp45.Models
{
    [Table("OF_Products")] // Sets the name of the table in the database
    public class Product
    {
        public Product() { }

        public Product(string name, decimal price, string description, string unit)
        {
            Name = name;
            UnitPrice = price;
            Description = description;
            Unit = unit;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public string Unit { get; set; }
        public string PictureFilename { get; set; }

        public static List<Product> GetSampleData()
        {
            var products = new List<Product>
            {
                // Procter & Gamble products
                new Product("Tide Laundry Detergent", 350.00m, "Original scent, 64 loads", "bottle"),
                new Product("Pampers Diapers", 999.00m, "Size 4, 120 count", "box"),
                new Product("Gillette Fusion Razor", 199.75m, "With 1 blade cartridge", "pack"),
            
                // Unilever products
                new Product("Dove Beauty Bar", 75.00m, "4 bars, moisturizing cream", "pack"),
                new Product("Hellmann's Mayonnaise", 120.00m, "Real, 30 oz", "jar"),
                new Product("Axe Body Spray", 190.00m, "Phoenix scent, 4 oz", "bottle"),

                // Nestle products
                new Product("Nescafe Instant Coffee", 170.00m, "Classic, 12 oz", "jar"),
                new Product("KitKat Chocolate Bar", 30.00m, "Milk chocolate, 1.5 oz", "bar"),
                new Product("Nestle Pure Life Water", 180.00m, "24-pack, 16.9 oz bottles", "pack"),
            
                // Add more products here if needed
            };

            return products;
        }
    }

}
