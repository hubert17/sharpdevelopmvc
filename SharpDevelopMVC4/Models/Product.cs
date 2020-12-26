using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SharpDevelopMVC4.Models
{
    [Table("OF_Products")]
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A Product Name is required")]
        [StringLength(160)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        public string Unit { get; set; }

        [ScaffoldColumn(false)]
        public string PictureFilename { get; set; } // Save as image file on disk

        public string MetaData { get; set; }

        
    }
}