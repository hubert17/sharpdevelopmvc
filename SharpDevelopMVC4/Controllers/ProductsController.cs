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
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int page = 0, int pageSize = 6)
        {
            var items = _db.Products.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
            	items = items.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));
            }
            
            if(page > 0)
            	items = items.Skip(pageSize * (page - 1)).Take(pageSize);
            
            return View(items.ToList());
            
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
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase fileUpload)
        {
            if(ModelState.IsValid)
            {
            	product.PictureFilename = fileUpload.SaveAsJpegFile(product.Name);
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
                _db.Entry(updatedProduct).Property(x => x.PictureFilename).IsModified = false;

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
