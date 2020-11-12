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

        [ForeignKey("Category")]
        public int? CategoryId { get; set; } // Nullable

        //[Range(0.01, 999.99, ErrorMessage = "Price must be between 0.01 and 999.99")]
        [Required(ErrorMessage = "Price is required")]
        public decimal UnitPrice { get; set; }

        public string Unit { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Picture { get; set; }

        [ScaffoldColumn(false)]
        public string PictureFilename { get; set; }

        public string MetaData { get; set; }

        public virtual Category Category { get; set; }
<<<<<<< Updated upstream

        
=======
        //public virtual List<OrderDetail> OrderDetails { get; set; }
>>>>>>> Stashed changes

    }
}