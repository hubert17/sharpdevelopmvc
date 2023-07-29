using ASPNETWebApp45.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETWebApp45.Controllers
{
    public class OrderController : Controller
    {
        MyApp45DbContext _db = new MyApp45DbContext();

        public ActionResult Index()
        {
            var orders = _db.Orders.ToList();
            return View(orders);
        }

        public ActionResult Get(int Id) // Order Id
        {
            var order = _db.Orders.Find(Id);
            return View(order);
        }

        [HttpPost]
        public ActionResult Create(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return RedirectToAction("Get", new { Id = order.Id });
        }

        [HttpPost]
        public ActionResult CreateItem(OrderItem orderItem)
        {
            _db.OrderItems.Add(orderItem);
            _db.SaveChanges();
            return RedirectToAction("Get", new { Id = orderItem.OrderId });
        }


        public ActionResult SeedSampleData()
        {
            _db.Customers.AddRange(Customer.GetSampleData());
            _db.Products.AddRange(Product.GetSampleData());
            _db.SaveChanges();

            TempData["alertbox"] = "Customer and Product table has been successfully seeded with sample data.";
            return RedirectToAction("Index");
        }
    }
}