using StockManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.ViewModels
{
    public class InactiveVM
    {        
       public List<Customer> Customers { get; set; }
       public List<Product> Product { get; set; }
        public List<ProductStock> OutOfStock { get; set; }
    }
}
