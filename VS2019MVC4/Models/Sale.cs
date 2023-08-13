using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASPNETWebApp45.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public string UserId { get; set; } // Cashier

        public DateTime SaleDate { get; set; }

        public DateTime CreateDate { get; set; }

        public string Notes { get; set; }

        [NotMapped]
        public decimal Total
        {
            get { return SaleItems != null ? SaleItems.Sum(t => t.Amount) : 0; }
        }

        public decimal Cash { get; set; }

        [NotMapped]
        public decimal Change
        {
            get { return Cash - Total; }
        }

        public List<SaleItem> SaleItems { get; set; }
        
    }
}