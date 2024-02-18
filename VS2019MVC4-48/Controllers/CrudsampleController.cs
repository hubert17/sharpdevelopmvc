using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPNETWebApp48.Models;

namespace ASPNETWebApp48.Controllers
{
	/// <summary>
	/// Description of ProductsController.
	/// </summary>
	public class CrudsampleController : Controller
	{
		MyApp48DbContext _db = new MyApp48DbContext();

		// GET: Products
		public ActionResult Index(string searchQry, int page = 0, int pageSize = 6)
		{
			var items = _db.Products.AsQueryable();

			if (!String.IsNullOrEmpty(searchQry))
				items = items.Where(s => s.Name.ToLower().Contains(searchQry.ToLower()));

			if (page > 0)
				items = items.Skip(pageSize * (page - 1)).Take(pageSize);

			ViewBag.SearchQry = searchQry;

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
				if (fileUpload != null)
					product.PictureFilename = fileUpload.SaveAsImageFile(product.Name);

				_db.Products.Add(product);
				_db.SaveChanges();

			}
			else
				// ModelState.AddModelError("", "There are some validation errors. Please check.");
				TempData["alertcard"] = "There are some validation errors. Please check and try again.";

			return RedirectToAction("Manage");
		}


		// GET: Products/Edit/5
		// [Authorize(Roles = "staff")]
		public ActionResult Edit(int id)
		{
			Product product = _db.Products.Find(id);

			if (product == null)
			{
				TempData["alertbox"] = "Product does not exist.";
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
				updatedProduct.PictureFilename = fileUpload.SaveAsImageFile(updatedProduct.Name);
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
			if (product != null)
			{
				_db.Products.Remove(product);
				_db.SaveChanges();
			}
			else
			{
				TempData["alertbox"] = "Product not found";
			}

			return RedirectToAction("Manage");
		}
	}
}