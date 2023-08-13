using ASPNETWebApp45.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETWebApp45.Controllers
{
    public class PosController : Controller
    {
        MyApp45DbContext _db = new MyApp45DbContext();

        // GET: Pos
        public ActionResult Index()
        {
            var model = new Dictionary<string, object>
            {
                { "ProductLookup", _db.Products.OrderBy(x => x.Name).ToList() },
                { "SaleDate", DateTime.Now },
                { "UserId", User.Identity.Name ?? "unknown-cashier" },
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(Sale sale)
        {
            sale.CreateDate = DateTime.Now;

            _db.Sales.Add(sale);
            _db.SaveChanges();

            TempData["alert-box"] = "Transaction has been saved.";

            return RedirectToAction("Index");
        }

        public ActionResult SeedSampleData()
        {
            _db.Products.AddRange(Product.GetSampleData());
            _db.SaveChanges();

            TempData["alertbox"] = "Product table has been successfully seeded with sample data.";
            return RedirectToAction("Index");
        }
    }
}