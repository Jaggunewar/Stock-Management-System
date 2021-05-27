using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StockManagementSystem.Models
{
   
    public class ProductStock
    {
        [Key]
        public int ProductStockID { get; set; }

        public int Quantity { get; set; }
        
        [ForeignKey("Product")]
        public int  ProductID { get; set; }
        public virtual Product Product { get; set; }

    }
}