using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockManagementSystem.DAL;
using StockManagementSystem.helper;
using StockManagementSystem.Models;
using StockManagementSystem.ViewModels;

namespace StockManagementSystem.Controllers
{
    [Authorize]

    public class PurchaseController : Controller
    {
        private StockContext db;
        public PurchaseController(StockContext _db)
        {
            db = _db;
        }
        // GET: Purchase
        public ActionResult Index(int? customerId)
        {


           
            List<PurchaseIndexVM> lstData = new List<PurchaseIndexVM>();
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {

                command.CommandText = @"SELECT p.ProductName, pu.VendorName,pu.BillNumber,pu.TotalPrice, 
                                        pd.PurchaseDetailID,pd.Quantity,pd.Remark,pd.Price,pd.PurchaseId from PurchaseDetails pd 
                                        join Purchase pu on  pd.PurchaseId=pu.PurchaseId 
                                        join Products p  on p.ProductId=pd.ProductId";
                db.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {

                    PurchaseIndexVM data;
                    while (result.Read())
                    {
                        data = new PurchaseIndexVM();

                        data.ProductName = result.GetString(0);
                        data.VendorName = result.GetString(1);
                        data.BillNumber = result.GetString(2);

                        // object totalPri =(double)result.GetValue(result.GetOrdinal("TotalPrice"));
                        // txtbalance.Text = value.ToString();

                        data.TotalPrice = (double)result.GetValue(result.GetOrdinal("TotalPrice"));
                        data.PurchaseDetailID = (int)(result.GetValue(4));
                        data.Quantity = (int)(result.GetValue(5));
                        data.Remark = !String.IsNullOrEmpty(result.GetString(6)) ? result.GetString(6) : "";
                        data.Price = (double)(result.GetValue(7));
                        data.PurchaseId = (int)(result.GetValue(8));
                        
                        lstData.Add(data);
                    }
                }


               
                return View(lstData);
            }
        }


        public ActionResult Detail(int? id)
        {
            List<PurchaseIndexVM> lstData = new List<PurchaseIndexVM>();
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {

                command.CommandText = @"SELECT p.ProductName, pu.VendorName,pu.BillNumber,pu.TotalPrice, 
                                        pd.PurchaseDetailID,pd.Quantity,pd.Remark,pd.Price, pu.PurchaseId,pu.PurchaseDate from PurchaseDetails pd 
                                        join Purchase pu on  pd.PurchaseId=pu.PurchaseId 
                                        join Products p  on p.ProductId=pd.ProductId 
                                        where pu.PurchaseId="+id.ToString();
                db.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {

                    PurchaseIndexVM data;
                    while (result.Read())
                    {
                        data = new PurchaseIndexVM();

                        data.ProductName = result.GetString(0);
                        data.VendorName = result.GetString(1);
                        data.BillNumber = result.GetString(2);

                        // object totalPri =(double)result.GetValue(result.GetOrdinal("TotalPrice"));
                        // txtbalance.Text = value.ToString();

                        data.TotalPrice = (double)result.GetValue(result.GetOrdinal("TotalPrice"));
                        data.PurchaseDetailID = (int)(result.GetValue(4));
                        data.Quantity = (int)(result.GetValue(5));
                        data.Remark = !String.IsNullOrEmpty(result.GetString(6)) ? result.GetString(6) : "";
                        data.Price = (double)(result.GetValue(7));
                        data.PurchaseId = (int)(result.GetValue(8));
                        data.PurchaseDate = (DateTime)(result.GetValue(9));

                        lstData.Add(data);
                    }
                }

                return View(lstData);
            }
        }
        public IActionResult Create()
        {

            var products = db.Products.ToList().Select(m => new DropDownItem { Id = m.ProductID, Name = m.ProductName }).ToList();
            List<PurchaseItemVM> lstPurchase = new List<PurchaseItemVM>() { 
                new PurchaseItemVM { 
                    Quantity = 0,
                    SubTotal = 0, 
                    ProductID = db.Products.FirstOrDefault().ProductID
                }
            };
            PurchaseVM objPurchase = new PurchaseVM
            {
                Items = lstPurchase,
                DropDownItems = products
            };
            return View(objPurchase);
        }
        [HttpPost]
        public IActionResult Create(PurchaseVM objPurchase)
        {
            var products = db.Products.ToList().Select(m => new DropDownItem { Id = m.ProductID, Name = m.ProductName }).ToList();
            objPurchase.DropDownItems = products;

            if (ModelState.IsValid)
            {
                Purchase purchase = new Purchase
                {
                    BillNumber = objPurchase.BillNumber,
                    PurchaseDate = DateTime.Now,
                    VendorName = objPurchase.VendorName,
                    TotalPrice = objPurchase.Items.Sum(m => m.SubTotal)

                };

                db.Purchase.Add(purchase);
                db.SaveChanges();


                foreach (var item in objPurchase.Items)
                {
                    PurchaseDetail purchaseDetail = new PurchaseDetail
                    {
                        PurchaseID = purchase.PurchaseID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Remark = ""
                    };
                    db.PurchaseDetails.Add(purchaseDetail);
                    db.SaveChanges();

                    ProductStock objStock = db.ProductStocks.FirstOrDefault(m => m.ProductID == item.ProductID);
                    objStock.Quantity = objStock.Quantity + item.Quantity;

                    db.SaveChanges();


                }
                return RedirectToAction("Index");

            }
            else
            {
                return View(objPurchase);
            }

        }
        public IActionResult Details()
        {

            return View();
        }
        //public ActionResult Create(string BillNumber, String VendorName, int? Quantity, int? ProductID)
        //{
        //    PurchaseVM purchase = new PurchaseVM
        //    {
        //        BillNumber = BillNumber,
        //        VendorName = VendorName,
        //        Quantity = Quantity!=null?(int)Quantity:0,
        //        ProductID =ProductID!=null? (int)ProductID:0,
        //        Items = new List<PurchaseItemVM>(),
        //        PurchaseDate = DateTime.Now
        //    };


        //    if (HttpContext.Session.GetObjectFromJson("Purchase", "Purchase") == null)
        //    {
        //        HttpContext.Session.SetObjectAsJson("Purchase", new List<PurchaseItemVM>());
        //    }


        //    var items = new List<DropDownItem>();

        //    if (db.Products.ToList().Count > 0)
        //    {
        //        items = db.Products.Select(m => new DropDownItem { Id = m.ProductID, Name = m.ProductID + "," + m.ProductName }).ToList();
        //        items.ToList().Insert(0, new DropDownItem { Id = 0, Name = "--Select--" });

        //    }
        //    else
        //    {
        //        items.ToList().Insert(0, new DropDownItem { Id = 0, Name = "--Select--" });

        //    }
        //    ViewBag.ItemId = new SelectList(items, "Id", "Name", 0);

        //    if (purchase != null)
        //    {
        //        //  ViewBag.CustomerId = new SelectList(customers, "Id", "Name", sale.CustomerId);
        //        ViewBag.ProductID = new SelectList(items, "Id", "Name", purchase.ProductID);
        //        purchase.PurchaseDate = purchase.PurchaseDate.Date < DateTime.Now.Date ? DateTime.Now : purchase.PurchaseDate;
        //    }
        //    purchase.Items = HttpContext.Session.GetObjectFromJson("Purchase", "Purchase");

        //    return View(purchase);
        //}
        //[HttpPost]
        //public ActionResult Create(string BillNumber, String VendorName)
        //{

        //    List<PurchaseItemVM> items = HttpContext.Session.GetObjectFromJson("Purchase","Purchase");
        //    if (items.Any())
        //    {
        //        var objSale = new Purchase
        //        {
        //         BillNumber=BillNumber,
        //           VendorName=VendorName,
        //           PurchaseDate=DateTime.Now,                    
        //            TotalPrice = (double)items.Sum(m => m.Quantity * m.Price)
        //        };

        //        db.Purchase.Add(objSale);
        //        db.SaveChanges();


        //        foreach (var item in items)
        //        {
        //            var objSaleItem = new PurchaseDetail
        //            {
        //                Price = item.Price,
        //                ProductID = item.ProductID,
        //                Quantity = item.Quantity,                        
        //                PurchaseID=objSale.PurchaseID,
        //                Remark=item.Remark                        
        //                //ItemId = item.Id,
        //                //SalesId = objSale.Id,
        //                //Quantity = item.Quantity,
        //                //SubTotal = item.SubTotal
        //            };

        //            db.PurchaseDetails.Add(objSaleItem);
        //            db.SaveChanges();

        //            var objStock = new ProductStock
        //            {
        //                ProductID = item.ProductID,
        //                Quantity = item.Quantity

        //            };

        //            db.ProductStocks.Add(objStock);
        //            db.SaveChanges();

        //            //var objItem = db.Products.FirstOrDefault(m => m.ProductID == item.Id);
        //            //// objItem.Quantity = objItem.Quantity - item.Quantity;
        //            //objItem.LastActive = DateTime.Now;
        //            //db.SaveChanges();
        //        }


        //        HttpContext.Session.SetObjectAsJson("Purchase", (List<PurchaseItemVM>)null);
        //        TempData["Message"] = "success";
        //        return Redirect("index");
        //    }
        //    ViewBag.ErrorMessage = "Error";
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult AddToCart(PurchaseVM sale)
        //{
        //    List<PurchaseItemVM> items = HttpContext.Session.GetObjectFromJson("Purchase","Purchase");
        //    if (items == null)
        //    {
        //        items = new List<PurchaseItemVM>();
        //    }
        //    if (!items.Any(m => m.ProductID == sale.PurchaseID))
        //    {
        //        // int quantity = db.ProductStocks.Where().Sum(m => m.Quantity);

        //        //var objItems = db.ProductStocks.Include("Product").Where(m => m.ProductID == sale.ItemId).ToList();

        //        // var objItem = db.Products.FirstOrDefault(m => m.ProductID == sale.ItemId);
        //        //if (objItems.Sum(m => m.Quantity) > sale.Quantity)
        //        //{
        //        var objItem = db.Products.FirstOrDefault(m => m.ProductID == sale.ProductID);
        //            PurchaseItemVM itemVM = new PurchaseItemVM
        //            {
        //                ProductID = sale.ProductID,
        //                Quantity = sale.Quantity,
        //                Remark = "",                       
        //                Price = sale.Price,
        //                SubTotal = sale.Quantity * sale.Price,
        //                Product=objItem
        //            };
        //            items.Add(itemVM);
        //            HttpContext.Session.SetObjectAsJson("Purchase", items);
        //            TempData["Message"] = "Item Successfully Added";

        //            return RedirectToAction("Create", new {BillNumber=sale.BillNumber,VendorName=sale.VendorName,Quantity=sale.Quantity,ProductID=sale.ProductID });
        //        //}
        //        //else
        //        //{
        //        //    TempData["ErrorMessage"] = "Not sufficient quantity. Please order less than " + objItems.Sum(m => m.Quantity).ToString() + " Nos.";
        //        //    //ViewBag.ItemId = new SelectList(db.Item.Select(m => new { Id = m.Id, Item = m.ItemCode + "," + m.ItemName }), "Id", "Item", ItemId);
        //        //    return RedirectToAction("Create", new { CustomerId = sale.CustomerId, ItemId = sale.ItemId, Quantity = sale.Quantity });
        //        //    // ViewBag.Quantity = Quantity;
        //        //    // return View();
        //        //}
        //    }
        //    return RedirectToAction("Create", new { BillNumber = sale.BillNumber, VendorName = sale.VendorName, Quantity = sale.Quantity, ProductID = sale.ProductID });


        //}
        //[HttpPost]
        //public ActionResult RemoveFromCart(int ProductID)
        //{
        //    List<PurchaseItemVM> sales = HttpContext.Session.GetObjectFromJson("Purchase","Purchase");
        //    if (sales.Any(m => m.ProductID == ProductID))
        //    {
        //        var item = sales.FirstOrDefault(m => m.ProductID == ProductID);
        //        sales.Remove(item);
        //        HttpContext.Session.SetObjectAsJson("Purchase", sales);
        //        return Json("success"); //RedirectToAction("Create", new { sale = new SalesVM() });

        //    }
        //    return Json("error");
        //}




    }
}