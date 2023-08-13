using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNETWebApp45.Models
{
    // [Table("OF_Products")] // Sets the name of the table to "OF_Product" in the database
    public class Product
    {
        public Product() { }

        public Product(string name, decimal price, string description, string unit)
        {
            Name = name;
            Description = description;
            UnitPrice = price;
            Unit = unit;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public string Unit { get; set; }
        
        public string PictureFilename { get; set; } // Save as image file on disk

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

                // Coca-Cola products
                new Product("Coca-Cola 1 Liter", 30.00m, "1 Liter bottle of Coca-Cola", "bottle"),
                new Product("Sprite 12 oz", 12.00m, "12 fl oz can of Sprite", "can"),
                new Product("Minute Maid Pulpy Orange", 25.00m, "Orange juice with pulp, 16.9 fl oz bottle", "bottle"),

                // Alcoholic beverages, decimal(18,2) is the default precision in EF6 code first
                new Product("Tanduay Rum 5 Years", 125.375m, "blended to a suave 750ml", "bottle"),
                new Product("Red Horse Beer", 118.0625m, "extra-strong beer, 1000ml", "bottle"),
                new Product("Emperador Light", 165.442m, "1L blended premium brandy", "bottle"),

                // Add more products here if needed
            };

            return products;
        }

    }
}
