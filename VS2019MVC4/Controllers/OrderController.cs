using ASPNETWebApp45.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var orders = _db.Orders.Include(x => x.OrderItems).ToList();

            ViewBag.ForCreatePartial = new Dictionary<string, object>
            {
                { "CustomerLookup", _db.Customers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList() },
                // { "KeyName", value },
                // Add more objects with their corresponding keys as needed
            };

            return View(orders);
        }

        public ActionResult Get(int Id = 3) // Order.Id
        {
            var order = _db.Orders.Where(x => x.Id == Id).Include(x => x.Customer).Include(x => x.OrderItems).SingleOrDefault();

            ViewBag.ForAddOrderItemPartial = new Dictionary<string, object>
            {
                { "ProductLookup", _db.Products.OrderBy(x => x.Name).ToList() },
                { "OrderId", order.Id }
            };

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
        public ActionResult Edit(Order updatedOrder)
        {
            _db.Entry(updatedOrder).State = EntityState.Modified;
            _db.SaveChanges();
            TempData["alertbox"] = "Order changes have been successfully saved.";
            return RedirectToAction("Get", new { Id = updatedOrder.Id });
        }

        [HttpPost]
        public ActionResult CreateItem(OrderItem orderItem)
        {
            _db.OrderItems.Add(orderItem);
            _db.SaveChanges();
            return RedirectToAction("Get", new { Id = orderItem.OrderId });
        }

        public ActionResult Delete(int id)
        {
            var orderItem = _db.OrderItems.Find(id);
            if (orderItem != null)
            {
                _db.OrderItems.Remove(orderItem);
                _db.SaveChanges();
            }

            return RedirectToAction("Get", new { Id = orderItem.OrderId });
        }

        public ActionResult GetProductPrice(int Id) // Product.Id
        {
            var unitPrice = _db.Products.Find(Id).UnitPrice;
            return Json(unitPrice, JsonRequestBehavior.AllowGet);
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