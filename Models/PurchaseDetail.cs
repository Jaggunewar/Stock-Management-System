using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StockManagementSystem.Models
{
   
    public class PurchaseDetail
    {
        [Key]
        public int PurchaseDetailID { get; set; }
        public int Quantity { get; set; }
        
        public string Remark { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("Purchase")]
        public int PurchaseID { get; set; }
        public virtual Purchase Purchase { get; set; }

        public double Price { get; set; }
    }
}