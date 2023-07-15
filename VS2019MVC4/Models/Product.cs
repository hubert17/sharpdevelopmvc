using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNETWebApp45.Models
{
    [Table("OF_Products")] // Sets the name of the table in the database
    public class Product
    {
        public Product() { }

        public Product(string name, decimal price)
        {
            Name = name;
            UnitPrice = price;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal UnitPrice { get; set; }
        
        public string Unit { get; set; }
        
        public string PictureFilename { get; set; } // Save as image file on disk
        
    }
}
