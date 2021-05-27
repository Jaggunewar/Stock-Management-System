using StockManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StockManagementSystem.ViewModels
{
    public class SalesVM
    {
       // public string CustomerName { get; set; }
       // public string  PhoneNumber { get; set; }
        public int CustomerId { get; set; }
       // public int ItemId { get; set; }
       // public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<DropDownItem> Items { get; set; }
        public List<SalesItemVM> SalesItems { get; set; }
        public double Total { get; set; }
    }
    public class SalesItemVM
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        // public string ItemCode { get; set; }
        //  public string ItemName { get; set; }
       // public string Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        //public DateTime PurchaseDate { get; set; }       
       // public DateTime ManufactureDate { get; set; }
       // public DateTime ExpiryDate { get; set; }
        public double SubTotal { get; set; }
        public string Remarks { get; set; }
    }

    public class SalesIndexVM
    {
        public string ProductName { get; set; }
        public DateTime SaleDate { get; set; }
        public int SalesID { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string  CustomerPhoneNumber { get; set; }       
        public double TotalAmount { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        //public IEnumerable<SalesDetails> Items { get; set; }
    }

    public class DropDownItem {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}