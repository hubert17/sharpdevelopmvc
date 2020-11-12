using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SharpDevelopMVC4.Models;
using X.PagedList;


namespace SharpDevelopMVC4.Controllers
{
    public class ProductsController : Controller
    {
        private SdMvc4DbContext _db = new SdMvc4DbContext();

        // GET: Products
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int pageSize = 6)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var items = _db.Products.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
            	items = items.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    items = items.OrderBy(s => s.UnitPrice);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.UnitPrice);
                    break;
                default:  // Name ascending 
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));


            //var items = db.Items.Include(i => i.Catagorie);
            //return View(await items.ToListAsync());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            Product product = _db.Products.Find(id);
            return View(product);
        }

        // GET: Products/Create
        //[Authorize(Roles = "staff")]
        public ActionResult Create()
        {
            var categories = _db.Categories
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            ViewBag.categories = categories;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase fileUpload)
        {
            if(ModelState.IsValid)
            {
            	product.PictureFilename = fileUpload.SaveAsJpegFile(product.Name, "products");
                _db.Products.Add(product);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        // GET: Products/Edit/5        
        public ActionResult Edit(int id)
        {
            Product product = _db.Products.Find(id);

            if (product == null)
            {
                TempData["msgAlert"] = "Product does not exist.";
                return RedirectToAction("Index");
            }

            ViewBag.categories = _db.Categories.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    Selected = s.Id == product.CategoryId ? true : false
                }).ToList();

            return View(product);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product updatedProduct, HttpPostedFileBase fileUpload)
        {
            _db.Entry(updatedProduct).State = EntityState.Modified;

            if (fileUpload != null) // Update picture
                updatedProduct.PictureFilename = fileUpload.SaveAsJpegFile(updatedProduct.Name, "products");
            else // Retain the current picture
                _db.Entry(updatedProduct).Property(x => x.Picture).IsModified = false;

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            Product product = _db.Products.Find(id);
            if(product != null)
            {
	            _db.Products.Remove(product);
	            _db.SaveChanges();	           
            }
            else
            {
            	TempData["msgAlert"] = "Product not found";
            }

            return RedirectToAction("Index");
        }

    }
}
