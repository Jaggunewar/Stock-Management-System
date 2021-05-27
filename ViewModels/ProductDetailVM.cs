using StockManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.ViewModels
{
    public class ProductDetailVM
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpireyDate { get; set; }
        public  Category Category { get; set; }
        public int Quantity { get; set; }
    }
}
