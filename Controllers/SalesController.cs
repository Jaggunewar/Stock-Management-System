using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockManagementSystem.DAL;
using StockManagementSystem.Models;
using StockManagementSystem.ViewModels;
using StockManagementSystem.helper;
using Microsoft.AspNetCore.Authorization;

namespace StockManagementSystem.Controllers
{
    [Authorize]

    public class SalesController : Controller
    {

        private StockContext db;
        public SalesController(StockContext _db)
        {
            db = _db;
        }


        public ActionResult Index(int? customerId)
        {
            ViewBag.CustomerId = customerId ?? 0;
           var customers= db.Customers.Select(m => new DropDownItem {Id=m.CustomerID, Name=m.CustomerName }).ToList();
            customers.Insert(0,new DropDownItem { Id = 0, Name = "--Select--" });
            ViewBag.CustomerIds = customers;
            List<SalesIndexVM> lstData = new List<SalesIndexVM>();
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {

                command.CommandText = @"SELECT p.ProductName,sa.SalesID, sa.SaleDate,sa.TotalAmount, 
                                       sd.Quantity,sd.Price ,c.CustomerName,c.CustomerPhoneNumber, c.CustomerId from SalesDetails sd 
                                        join Sales sa on  sd.SalesID=sa.SalesID 
                                        join Products p  on p.ProductID=sd.ProductID
										join Customers c on sa.CustomerID=c.CustomerID";
                db.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {

                    SalesIndexVM data;
                    while (result.Read())
                    {
                        data = new SalesIndexVM();

                        data.ProductName = result.GetString(0);
                        data.SalesID = (int)result.GetValue(1);
                        data.SaleDate = (DateTime)result.GetValue(2);

                        // object totalPri =(double)result.GetValue(result.GetOrdinal("TotalPrice"));
                        // txtbalance.Text = value.ToString();

                        data.TotalAmount = (double)result.GetValue(result.GetOrdinal("TotalAmount"));
                        data.Quantity = (int)(result.GetValue(4));
                        data.Price = (double)(result.GetValue(5));
                        data.CustomerName = !String.IsNullOrEmpty(result.GetString(6)) ? result.GetString(6) : "";
                        data.CustomerPhoneNumber = result.GetString(7);
                         data.CustomerId = (int)(result.GetValue(8));
                        lstData.Add(data);
                    }
                }

                if (customerId != null)
                {
                    DateTime monthAgo = DateTime.Now.AddMonths(-1);
                    //ViewBag.CustomerId = new SelectList(db.Customers.ToList(), "CustomerId", "CustomerName", customerId.ToString());
                    lstData = lstData.Where(m => m.CustomerId == (int)customerId&& m.SaleDate>monthAgo).ToList();
                }
                return View(lstData);
                //return View();

            }
        } public ActionResult Details(int? id)
        {

            List<SalesIndexVM> lstData = new List<SalesIndexVM>();
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {

                command.CommandText = @"SELECT p.ProductName,sa.SalesID, sa.SaleDate,sa.TotalAmount, 
                                       sd.Quantity,sd.Price ,c.CustomerName,c.CustomerPhoneNumber from SalesDetails sd 
                                        join Sales sa on  sd.SalesID=sa.SalesID 
                                        join Products p  on p.ProductID=sd.ProductID
										join Customers c on sa.CustomerID=c.CustomerID where sa.SalesID="+id.ToString();
                db.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {

                    SalesIndexVM data;
                    while (result.Read())
                    {
                        data = new SalesIndexVM();

                        data.ProductName = result.GetString(0);
                        data.SalesID = (int)result.GetValue(1);
                        data.SaleDate = (DateTime)result.GetValue(2);

                        // object totalPri =(double)result.GetValue(result.GetOrdinal("TotalPrice"));
                        // txtbalance.Text = value.ToString();

                        data.TotalAmount = (double)result.GetValue(result.GetOrdinal("TotalAmount"));
                        data.Quantity = (int)(result.GetValue(4));
                        data.Price = (double)(result.GetValue(5));
                        data.CustomerName = !String.IsNullOrEmpty(result.GetString(6)) ? result.GetString(6) : "";
                        data.CustomerPhoneNumber = result.GetString(7);
                        // data.Price = (double)(result.GetValue(7));
                        lstData.Add(data);
                    }
                }

                return View(lstData);
                //return View();

            }
        }
        public IActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Customers.ToList(), "CustomerID", "CustomerName");
            var products = db.ProductStocks.Include(m=>m.Product).ToList().Select(m => new DropDownItem { Id = m.ProductID, Name = m.Product.ProductName,Price=m.Product.Price }).ToList();
            List<SalesItemVM> objSale = new List<SalesItemVM>() { new SalesItemVM { Quantity = 0, SubTotal = 0, ProductID = db.Products.FirstOrDefault().ProductID } };
            SalesVM objPurchase = new SalesVM
            {
                Items = products,
                SalesItems = objSale
            };
            return View(objPurchase);
        }
        [HttpPost]
        public IActionResult Create(SalesVM objPurchase)
        {
            ViewBag.CustomerId = new SelectList(db.Customers.ToList(), "CustomerID", "CustomerName",objPurchase.CustomerId);
            var products = db.Products.ToList().Select(m => new DropDownItem { Id = m.ProductID, Name = m.ProductName }).ToList();
            objPurchase.Items = products;

            if (ModelState.IsValid)
            {
                Sales purchase = new Sales
                {
                    CustomerID = objPurchase.CustomerId,
                    SaleDate = DateTime.Now,
                    TotalAmount = objPurchase.SalesItems.Sum(m => m.SubTotal)
                };

                db.Sales.Add(purchase);
                db.SaveChanges();


                Customer customer = db.Customers.FirstOrDefault(m => m.CustomerID ==objPurchase.CustomerId);
                customer.LastActive = DateTime.Now;
                db.SaveChanges();

                foreach (var item in objPurchase.SalesItems)
                {
                    SalesDetails purchaseDetail = new SalesDetails
                    {
                        SalesID = purchase.SalesID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.Price,                        
                        Remarks = ""
                    };
                    db.SalesDetails.Add(purchaseDetail);
                    db.SaveChanges();

                    ProductStock objStock = db.ProductStocks.FirstOrDefault(m => m.ProductID == item.ProductID);
                    objStock.Quantity = objStock.Quantity - item.Quantity;
                    db.SaveChanges();

                    Product product = db.Products.FirstOrDefault(m => m.ProductID == item.ProductID);
                    product.LastActive = DateTime.Now;
                    db.SaveChanges();



                }
                return RedirectToAction("Index");

            }
            else
            {
                return View(objPurchase);
            }

        }
        // GET: Sales
        //public ActionResult Index(int? customerName)
        //{
        //    var customers = db.Customers.Select(m => new { CustomerName = m.CustomerPhoneNumber + "," + m.CustomerName, Id = m.CustomerID });
        //    ViewBag.customerName = new SelectList(customers, "Id", "CustomerName");
        //    var sales = db.Sales.Include("Customer").AsEnumerable();
        //    var objSales = new List<SalesIndexVM>();

        //    if (customerName != null)
        //    {
        //        ViewBag.customerName = new SelectList(customers, "Id", "CustomerName", customerName);
        //        DateTime date = DateTime.Now.AddMonths(-1);
        //        sales = sales.Where(m => m.CustomerID == (int)customerName && m.SaleDate >= date);
        //        //objSale.Customer = db.Customers.FirstOrDefault(m => m.CustomerID == (int)customerName);
        //        //objSale.CustomerID = (int)customerName;
        //    }

        //    foreach (var item in sales.ToList())
        //    {
        //        var objSale = new SalesIndexVM();
        //        objSale.SalesID = item.SalesID;
        //        objSale.Items = db.SalesDetails.Include(m=>m.Product).Where(m => m.SalesID == item.SalesID).AsEnumerable();
        //        objSale.Customer = item.Customer;
        //        objSale.CustomerID = item.CustomerID;
        //        objSale.SaleDate = DateTime.Now;

        //        objSales.Add(objSale);
        //    }
        //    return View(objSales.AsEnumerable());
        //}


        //public ActionResult Details(int? Id)
        //{
        //    if (Id == null)
        //    {
        //        return BadRequest();
        //    }
        //    else
        //    {
        //        Sales objSale = db.Sales.Include(m=>m.Customer).FirstOrDefault(m => m.SalesID == (int)Id);
        //        List<SalesDetails> lstSaleItems = db.SalesDetails.Include(m=>m.Product).Where(m => m.SalesID == (int)Id).ToList();

        //        SalesVM objSaleVM = new SalesVM
        //        {
        //            Total=lstSaleItems.Sum(m=>(m.Quantity*m.Price)),
        //            PurchaseDate = objSale.SaleDate,
        //            CustomerName = objSale.Customer.CustomerName,
        //            PhoneNumber = objSale.Customer.CustomerPhoneNumber,
        //            Items = lstSaleItems.Select(m => new SalesItemVM {
        //                ItemName = m.Product.ProductName,
        //                Price = m.Price,
        //                Quantity = m.Quantity,
        //                SubTotal = m.Quantity * m.Price
        //            }).ToList()
        //        };

        //        return View(objSaleVM);
        //    }
        //}
        //public ActionResult Create(int? CustomerId, int? Quantity, int? ItemId)
        //{
        //    SalesVM sale = new SalesVM
        //    {
        //        CustomerId = CustomerId != null ? (int)CustomerId : 0,
        //        ItemId = ItemId != null ? (int)ItemId : 0,
        //        Items = new List<SalesItemVM>(),
        //        PurchaseDate = DateTime.Now,
        //    };

        //    if (HttpContext.Session.GetObjectFromJson("Cart") == null)
        //    {
        //      HttpContext.Session.SetObjectAsJson("Cart", new List<SalesItemVM>());
        //    }

        //    var customers = new List<DropDownItem>();
        //    var items = new List<DropDownItem>();
        //    if (db.Customers.ToList().Count > 0)
        //    {
        //        customers = db.Customers.Select(m => new DropDownItem { Id = m.CustomerID, Name = m.CustomerPhoneNumber + ", " + m.CustomerName }).ToList();
        //        customers.ToList().Insert(0, new
        //        DropDownItem
        //        {
        //            Id = 0,
        //            Name = "--Select--"
        //        });
        //    }
        //    else
        //    {
        //        customers.ToList().Insert(0, new
        //            DropDownItem
        //        {
        //            Id = 0,
        //            Name = "--Select--"
        //        });
        //    }

        //    if (db.Products.ToList().Count > 0)
        //    {
        //        items = db.Products.Select(m => new DropDownItem { Id = m.ProductID, Name = m.ProductID + "," + m.ProductName }).ToList();
        //        items.ToList().Insert(0, new DropDownItem { Id = 0, Name = "--Select--" });

        //    }
        //    else
        //    {
        //        items.ToList().Insert(0, new DropDownItem { Id = 0, Name = "--Select--" });

        //    }
        //    ViewBag.CustomerId = new SelectList(customers, "Id", "Name", 0);
        //    ViewBag.ItemId = new SelectList(items, "Id", "Name", 0);

        //    if (sale != null)
        //    {
        //        ViewBag.CustomerId = new SelectList(customers, "Id", "Name", sale.CustomerId);
        //        ViewBag.ItemId = new SelectList(items, "Id", "Name", sale.ItemId);
        //        sale.PurchaseDate = sale.PurchaseDate.Date < DateTime.Now.Date ? DateTime.Now : sale.PurchaseDate;
        //    }
        //    sale.Items =HttpContext.Session.GetObjectFromJson("Cart");

        //    return View(sale);
        //}
        //[HttpPost]
        //public ActionResult Create(int? CustomerId)
        //{

        //    List<SalesItemVM> items = HttpContext.Session.GetObjectFromJson("Cart");
        //    if (items.Any())
        //    {
        //        var objSale = new Sales
        //        {
        //            CustomerID = (int)CustomerId,
        //            SaleDate = DateTime.Now,
        //            TotalAmount = (double)items.Sum(m => m.Quantity * m.Price)
        //        };

        //        db.Sales.Add(objSale);
        //        db.SaveChanges();

        //        var objCustomer = db.Customers.FirstOrDefault(m => m.CustomerID == (int)CustomerId);
        //        objCustomer.LastActive = DateTime.Now;
        //        db.SaveChanges();

        //        foreach (var item in items)
        //        {
        //            var objSaleItem = new SalesDetails
        //            {
        //                Price = item.Price,
        //                ProductID = item.Id,
        //                Quantity = item.Quantity,
        //                SalesID = objSale.SalesID,
        //                Remarks = item.Remarks
        //                //ItemId = item.Id,
        //                //SalesId = objSale.Id,
        //                //Quantity = item.Quantity,
        //                //SubTotal = item.SubTotal
        //            };

        //            db.SalesDetails.Add(objSaleItem);
        //            db.SaveChanges();

        //            var objStock = new ProductStock
        //            {
        //                ProductID = item.Id,
        //                Quantity = -item.Quantity

        //            };

        //            db.ProductStocks.Add(objStock);
        //            db.SaveChanges();

        //            var objItem = db.Products.FirstOrDefault(m => m.ProductID == item.Id);
        //            // objItem.Quantity = objItem.Quantity - item.Quantity;
        //            objItem.LastActive = DateTime.Now;
        //            db.SaveChanges();
        //        }


        //        HttpContext.Session.SetObjectAsJson("Cart", (List<SalesItemVM>)null);
        //        TempData["Message"] = "success";
        //        return Redirect("index");
        //    }
        //    ViewBag.ErrorMessage = "Error";
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult AddToCart(SalesVM sale)
        //{
        //    List<SalesItemVM> items = HttpContext.Session.GetObjectFromJson("Cart");
        //    if (items == null)
        //    {
        //        items = new List<SalesItemVM>();
        //    }
        //    if (!items.Any(m => m.Id == sale.ItemId))
        //    {
        //        // int quantity = db.ProductStocks.Where().Sum(m => m.Quantity);

        //        var objItems = db.ProductStocks.Include("Product").Where(m => m.ProductID == sale.ItemId).ToList();

        //        // var objItem = db.Products.FirstOrDefault(m => m.ProductID == sale.ItemId);
        //        if (objItems.Sum(m => m.Quantity) > sale.Quantity)
        //        {
        //            var objItem = objItems.FirstOrDefault();
        //            SalesItemVM itemVM = new SalesItemVM
        //            {
        //                Id = sale.ItemId,
        //                Quantity = sale.Quantity,
        //                Description = objItem.Product.Description,
        //                //  ItemCode = objItem.ItemCode,
        //                ItemName = objItem.Product.ProductName,
        //                Price = objItem.Product.Price,
        //                SubTotal = sale.Quantity * objItem.Product.Price
        //            };
        //            items.Add(itemVM);
        //            HttpContext.Session.SetObjectAsJson("Cart", items);
        //            TempData["Message"] = "Item Successfully Added";

        //            return RedirectToAction("Create", new { CustomerId = sale.CustomerId, ItemId = sale.ItemId, Quantity = sale.Quantity });
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Not sufficient quantity. Please order less than " + objItems.Sum(m => m.Quantity).ToString() + " Nos.";
        //            //ViewBag.ItemId = new SelectList(db.Item.Select(m => new { Id = m.Id, Item = m.ItemCode + "," + m.ItemName }), "Id", "Item", ItemId);
        //            return RedirectToAction("Create", new { CustomerId = sale.CustomerId, ItemId = sale.ItemId, Quantity = sale.Quantity });
        //            // ViewBag.Quantity = Quantity;
        //            // return View();
        //        }
        //    }
        //    return RedirectToAction("Create", new { CustomerId = sale.CustomerId, ItemId = sale.ItemId, Quantity = sale.Quantity });


        //}
        //[HttpPost]
        //public ActionResult RemoveFromCart(int itemId)
        //{
        //    List<SalesItemVM> sales = HttpContext.Session.GetObjectFromJson("Cart");
        //    if (sales.Any(m => m.Id == itemId))
        //    {
        //        var item = sales.FirstOrDefault(m => m.Id == itemId);
        //        sales.Remove(item);
        //        HttpContext.Session.SetObjectAsJson("Cart", sales);
        //        return Json("success"); //RedirectToAction("Create", new { sale = new SalesVM() });

        //    }
        //    return Json("error");
        //}

    }
}