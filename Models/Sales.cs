using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StockManagementSystem.Models
{
    
    public class Sales
    {
        [Key]
        public int SalesID { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        public double TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}