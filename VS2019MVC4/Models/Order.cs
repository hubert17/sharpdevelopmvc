using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASPNETWebApp45.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        
        public DateTime OrderDate { get; set; }
        public string Notes { get; set; }

    }
}