using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockManagementSystem.DAL;
using StockManagementSystem.ViewModels;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly StockContext db;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext appDb;

        public HomeController(ILogger<HomeController> logger, StockContext _db, ApplicationDbContext _appDb)
        {
            _logger = logger;
            db = _db;
            appDb = _appDb;

        }

        public IActionResult Index()
        {
            //DateTime monthAgo = DateTime.Now.AddMonths(-1);
            //var inActiveUser = db.Customers.Where(m => m.LastActive < monthAgo).ToList();
            //var inActiveProduct = db.Products.Include(m => m.Category).Where(m => m.LastActive < monthAgo).ToList();
            //var outOfStock = db.ProductStocks.Include(m => m.Product).Where(m => m.Quantity < 10).ToList();

            //InactiveVM inactive = new InactiveVM
            //{
            //    Customers = inActiveUser,
            //    Product = inActiveProduct,
            //    OutOfStock = outOfStock
            //};

            return View();
        }
        public IActionResult InActiveUser()
        {
            DateTime monthAgo = DateTime.Now.AddMonths(-1);
            var inActiveUser = db.Customers.Where(m => m.LastActive < monthAgo).ToList();


            return View(inActiveUser);
        }

        public IActionResult InActiveProduct()
        {
            DateTime monthAgo = DateTime.Now.AddMonths(-1);
            var inActiveProduct = db.Products.Include(m => m.Category).Where(m => m.LastActive < monthAgo).ToList();

            return View(inActiveProduct);
        }

        public IActionResult RemoveInActiveProduct()
        {
            DateTime monthAgo = DateTime.Now.AddMonths(-1);
            var inActiveProduct = db.ProductStocks.Include(m => m.Product).Where(m => m.Product.LastActive < monthAgo).ToList();

            foreach (var item in inActiveProduct)
            {
                var objStock = db.ProductStocks.Find(item.ProductStockID);
                objStock.Quantity = 0;
                db.SaveChanges();
            }

            return RedirectToAction("InActiveProduct");

        }

        public IActionResult OutOfStock(string sortOrder)
        {
            DateTime monthAgo = DateTime.Now.AddMonths(-1);
            var outOfStock = db.ProductStocks.Include(m => m.Product).
                             Include(m => m.Product.Category).Where(m => m.Quantity < 10).ToList();


            ViewData["NameSortParm"] =sortOrder== "name" ? "name_desc" : "name";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["QuantitySortParm"] = sortOrder == "qty" ? "qty_desc" : "qty";

            switch (sortOrder)
            {

                case "name_desc":
                    outOfStock = outOfStock.OrderByDescending(s => s.Product.ProductName).ToList();
                    break;
                case "name":
                    outOfStock = outOfStock.OrderBy(s => s.Product.ProductName).ToList();
                    break;
                case "date_desc":
                    outOfStock = outOfStock.OrderByDescending(s => s.Product.LastActive).ToList();
                    break;
                case "date":
                    outOfStock = outOfStock.OrderBy(s => s.Product.LastActive).ToList();
                    break;
                case "qty_desc":
                    outOfStock = outOfStock.OrderByDescending(s => s.Quantity).ToList();
                    break;
                case "qty":
                    outOfStock = outOfStock.OrderBy(s => s.Quantity).ToList();
                    break;

                default:
                    outOfStock = outOfStock.OrderBy(s => s.Product.ProductName).ToList();
                    break;
            }

            return View(outOfStock);
        }

        public async Task<IActionResult> AllUsers()
        {
            var Users = await appDb.Users.Select(m => new AdminVM { Name = m.UserName, Email = m.Email, Id = m.Id, PhoneNumber = m.PhoneNumber }).ToListAsync();
            return View(Users);
        }

        public ActionResult UpdateUser(string id)
        {
            // var user = UserManager.FindById(User.Identity.GetUserId());
           
            var user = appDb.Users.FirstOrDefault(m => m.Id == id);
            if (user != null)
            {
                var userVM = new AdminVM
                {
                    Id = id,
                    Name = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                return View(userVM);
            }
            else {
                return RedirectToAction("allUsers");
            }
        }

        [HttpPost]
        public ActionResult UpdateUser(AdminVM objUser)
        {
            var User = appDb.Users.FirstOrDefault(m => m.Id == objUser.Id);
            User.UserName = objUser.Name;
            User.Email = objUser.Email;
            User.PhoneNumber = objUser.PhoneNumber;
            appDb.SaveChanges();
            return RedirectToAction("AllUsers");
               
        }

        // public ActionResult ChangePassword(string id)
        //{
        //    var user = appDb.Users.FirstOrDefault(m => m.Id == id);
        //    if (user != null)
        //    {
        //        var pass = new ChangePasswordVM
        //        {
        //            Id = id                   
        //        };
        //        return View(pass);
        //    }
        //    else {
        //        return RedirectToAction("allUsers");
        //    }
        //}

        //[HttpPost]
        //public ActionResult ChangePassword(ChangePasswordVM objUser)
        //{
        //    var User = appDb.Users.FirstOrDefault(m => m.Id == objUser.Id);
        //   var passToken =appDb.User
        //    User.UserName = objUser.Name;
        //    User.Email = objUser.Email;
        //    User.PhoneNumber = objUser.PhoneNumber;
        //    appDb.SaveChanges();
        //    return RedirectToAction("AllUsers");
               
        //}



        [HttpPost]
        public ActionResult GetOutOfStock()
        { 
            var outOfStock = db.ProductStocks.Include(m => m.Product).Where(m => m.Quantity < 10).ToList();
            return Json(outOfStock.Count.ToString());
        }
        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
