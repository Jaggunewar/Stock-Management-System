using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StockManagementSystem.Models
{
    
    public class Purchase
    {
        public int PurchaseID { get; set; }
        public string VendorName { get; set; }
        public string BillNumber { get; set; }
        public double TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }

    }
}