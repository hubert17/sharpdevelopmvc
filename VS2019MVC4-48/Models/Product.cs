using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNETWebApp48.Models
{
    // [Table("OF_Products")] // Sets the name of the table to "OF_Product" in the database
    public class Product
    {
        public Product() { }

        public Product(string name, decimal price, string unit)
        {
            Name = name;
            UnitPrice = price;
            Unit = unit;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public string Unit { get; set; }
        
        public string PictureFilename { get; set; } // Save as image file on disk

    }
}
