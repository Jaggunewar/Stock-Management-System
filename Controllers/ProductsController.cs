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
using StockManagementSystem.Models;
using WebApplication1.ViewModels;

namespace StockManagementSystem.Controllers
{
    [Authorize]

    public class ProductsController : Controller
    {
        private StockContext db;

        public ProductsController(StockContext _db)
        {
            db = _db;
        }

        // GET: Products
        public ActionResult Index(string productName)
        {
            var products = db.Products.Include(p => p.Category).ToList();
            if (!String.IsNullOrEmpty(productName))
            {
               products = products.Where(m => m.ProductName.ToLower()
                                     .Contains(productName.ToLower())).ToList();
            }
            return View(products.ToList());

        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }   
            var product = db.Products.Include(m=>m.Category).FirstOrDefault(m=>m.ProductID==id);
            var productStock = db.ProductStocks.FirstOrDefault(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            ProductDetailVM productVM = new ProductDetailVM
            {
                Category = product.Category,
                Description = product.Description,
                ExpireyDate = product.ExpireyDate,
                ManufactureDate = product.ManufactureDate,
                Price = product.Price,
                Quantity = productStock.Quantity,
                ProductName = product.ProductName
            };
            return View(productVM);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                ProductStock objstock = new ProductStock               
                {
                    ProductID = product.ProductID,
                    Quantity = 0
                };

                db.ProductStocks.Add(objstock);
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return  BadRequest();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
