using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SharpDevelopMVC4.Models
{
    [Table("OF_Categories")]
    public class Category
    {       
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(160)]
        public string Name { get; set; }

        public string MetaData { get; set; }

        public virtual ICollection<Product> Items { get; set; }
    }
}