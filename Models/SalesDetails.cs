using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models
{
    public class SalesDetails
    {
        [Key]
        public int SalesDetailID { get; set; }

        [ForeignKey("Sales")]
        public int SalesID { get; set; }
        public virtual Sales Sales { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public string Remarks { get; set; }
    }
}
