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
            var newSale = new Sale();
            newSale.SaleDate = DateTime.Now;
            newSale.UserId = User.Identity.Name ?? "unknown-cashier";

            _db.Sales.Add(newSale);
            _db.SaveChanges();

            ViewBag.SaleId = newSale.Id;

            return View();
        }

        public ActionResult Add(List<SaleDetail> saleDetails)
        {
            _db.SaleDetails.AddRange(saleDetails);
            _db.SaveChanges();

            TempData["alert-box"] = "Transaction has been saved.";

            return RedirectToAction("Index");
        }
    }
}