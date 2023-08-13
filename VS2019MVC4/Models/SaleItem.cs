using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASPNETWebApp45.Models
{
    public class SaleItem
    {
        public int Id { get; set; }

        // With the virtual keyword on the Order navigation property, 
        // Entity Framework will employ lazy-loading, 
        // and the related Order entity will only be fetched from the database 
        // when you explicitly access the Order property in your code. 
        // This can help improve performance by avoiding unnecessary loading of related data when not needed.
        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }
        public int SaleId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal Amount
        {
            get { return Quantity * UnitPrice; }
        }
    }
}