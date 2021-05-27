using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StockManagementSystem.Models
{
  
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpireyDate { get; set; } 
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
        public DateTime LastActive { get; set; }

    }
}