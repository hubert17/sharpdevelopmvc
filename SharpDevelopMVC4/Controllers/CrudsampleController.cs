using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPNETWebApp45.Models;

namespace ASPNETWebApp45.Controllers
{
	public class CrudsampleController : Controller
	{
		MyApp45DbContext _db = new MyApp45DbContext();

		// GET: Products
		public ActionResult Index(string searchQry, int page = 1, int pageSize = 20)
		{
			var items = _db.Products.AsQueryable();

			if (!String.IsNullOrEmpty(searchQry))
				items = items.Where(s => s.Name.Contains(searchQry));
            
			int totalItems = items.Count();
			int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
			if (page < 1) page = 1;
			if (page > totalPages && totalPages > 0) page = totalPages;

			var pagedItems = items.OrderBy(p => p.Id).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            
			ViewBag.SearchQry = searchQry;
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			
			return View(pagedItems);            
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
				if (fileUpload != null)
					product.PictureFilename = fileUpload.SaveAsImageFile(product.Name);
				
				_db.Products.Add(product);
				_db.SaveChanges();
				
				TempData["alertbox"] = "Product '" + product.Name + "' created successfully.";
				return RedirectToAction("Manage");
			} 
			
			TempData["alertcard"] = "There are some validation errors. Please check and try again.";
			foreach (var modelState in ModelState.Values)
			{
				foreach (var error in modelState.Errors)
				{
					ModelState.AddModelError("", error.ErrorMessage);
				}
			}
			return View("Manage", _db.Products.ToList());			
		}


		// GET: Products/Edit/5
		// [Authorize(Roles = "staff")]
		public ActionResult Edit(int id)
		{
			Product product = _db.Products.Find(id);

			if (product == null) {
				TempData["alertbox"] = "Product does not exist.";
				return RedirectToAction("Manage");
			}

			return View(product);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Product updatedProduct, HttpPostedFileBase fileUpload)
		{
			if (ModelState.IsValid)
			{
				_db.Entry(updatedProduct).State = EntityState.Modified;

				if (fileUpload != null) // Update picture
					updatedProduct.PictureFilename = fileUpload.SaveAsImageFile(updatedProduct.Name);
				else // Retain the current picture
					_db.Entry(updatedProduct).Property(x => x.PictureFilename).IsModified = false;

				_db.SaveChanges();

				TempData["alertbox"] = "Product '" + updatedProduct.Name + "' updated successfully.";
				return RedirectToAction("Manage");
			}

			TempData["alertcard"] = "There are some validation errors. Please check and try again.";
			foreach (var modelState in ModelState.Values)
			{
				foreach (var error in modelState.Errors)
				{
					ModelState.AddModelError("", error.ErrorMessage);
				}
			}
			return View(updatedProduct);
		}

		// GET: Products/Delete/5
		// [Authorize(Roles = "staff")]
		public ActionResult Delete(int id)
		{
			Product product = _db.Products.Find(id);
			if (product != null) {
				string name = product.Name;
				_db.Products.Remove(product);
				_db.SaveChanges();
				TempData["alertbox"] = "Product '" + name + "' deleted successfully.";
			} else {
				TempData["alertbox"] = "Product not found";
			}

			return RedirectToAction("Manage");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}