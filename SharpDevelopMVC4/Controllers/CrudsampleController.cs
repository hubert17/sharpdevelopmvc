using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharpDevelopMVC4.Models;

namespace SharpDevelopMVC4.Controllers
{
	/// <summary>
	/// Description of ProductsController.
	/// </summary>
	public class CrudsampleController : Controller
	{
		private readonly SdMvc4DbContext _db = new SdMvc4DbContext();

		// GET: Products
		public ActionResult Index(string searchString, int page = 0, int pageSize = 6)
		{
			var items = _db.Products.AsQueryable();

			if (!String.IsNullOrEmpty(searchString))
				items = items.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));
            
			if (page > 0)
				items = items.Skip(pageSize * (page - 1)).Take(pageSize);
            
			return View(items.ToList());
            
		}
		
		// [Authorize(Roles = "staff")]
		public ActionResult Manage()
		{
			var items = _db.Products.ToList();           
			return View(items);            
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
			var product = new Product();
			return View(product);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Product product, HttpPostedFileBase fileUpload)
		{
			if (ModelState.IsValid) 
			{
				if(fileUpload != null)
					product.PictureFilename = fileUpload.SaveAsJpegFile(product.Name);
				
				_db.Products.Add(product);
				_db.SaveChanges();
				return RedirectToAction("Manage");
			}
			else
				return View(product);
		}


		// GET: Products/Edit/5
		// [Authorize(Roles = "staff")]
		public ActionResult Edit(int id)
		{
			Product product = _db.Products.Find(id);

			if (product == null) {
				TempData["msgAlert"] = "Product does not exist.";
				return RedirectToAction("Manage");
			}

			return View(product);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Product updatedProduct, HttpPostedFileBase fileUpload)
		{
			_db.Entry(updatedProduct).State = EntityState.Modified;

			if (fileUpload != null) // Update picture
                updatedProduct.PictureFilename = fileUpload.SaveAsJpegFile(updatedProduct.Name);
			else // Retain the current picture
                _db.Entry(updatedProduct).Property(x => x.PictureFilename).IsModified = false;

			_db.SaveChanges();

			return RedirectToAction("Manage");
		}

		// GET: Products/Delete/5
		// [Authorize(Roles = "staff")]
		public ActionResult Delete(int id)
		{
			Product product = _db.Products.Find(id);
			if (product != null) {
				_db.Products.Remove(product);
				_db.SaveChanges();	           
			} else {
				TempData["msgAlert"] = "Product not found";
			}

			return RedirectToAction("Manage");
		}
	}
}