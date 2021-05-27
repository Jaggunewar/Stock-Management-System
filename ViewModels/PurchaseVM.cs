using StockManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockManagementSystem.ViewModels
{
    public class PurchaseVM
    {
        public int PurchaseID { get; set; }
        public string VendorName { get; set; }
        public string BillNumber { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<PurchaseItemVM> Items { get; set; }
        public List<DropDownItem> DropDownItems { get; set; }

    }

    public class PurchaseItemVM {
      //  public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
        public int ProductID { get; set; }
        public  Product Product { get; set; }
        public double Price { get; set; }
        public double  SubTotal { get; set; }
    }

    public class PurchaseIndexVM {

        //Purchase
        public int PurchaseId { get; set; }
        public string VendorName { get; set; }
        public string BillNumber { get; set; }
        public double TotalPrice { get; set; }

        //Purchase details
        public int PurchaseDetailID { get; set; }
        public int Quantity { get; set; }

        public string Remark { get; set; }   

        public double Price { get; set; }

        //product
        public string ProductName { get; set; }
        public string SubTotal { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
    //public class DropDownItem {
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //} 

}