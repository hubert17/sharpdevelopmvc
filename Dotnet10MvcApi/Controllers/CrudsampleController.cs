using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet10MvcApi.Data;
using Dotnet10MvcApi.Helpers;
using Dotnet10MvcApi.Models;

namespace Dotnet10MvcApi.Controllers
{
    public class CrudsampleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CrudsampleController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchQry, int page = 1, int pageSize = 20)
        {
            var items = _db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchQry))
                items = items.Where(s => s.Name.Contains(searchQry));
            
            int totalItems = await items.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedItems = await items.OrderBy(p => p.Id).Skip(pageSize * (page - 1)).Take(pageSize).ToListAsync();
            
            ViewBag.SearchQry = searchQry;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            
            return View(pagedItems);            
        }
        
        // [Authorize(Roles = "staff")]
        public async Task<IActionResult> Manage()
        {
            var items = await _db.Products.ToListAsync();           
            return View(items);            
        }		

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            Product? product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Products/Create
        //[Authorize(Roles = "staff")]
        public IActionResult Create()
        {
            var product = new Product();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? fileUpload)
        {
            if (ModelState.IsValid) 
            {
                if (fileUpload != null)
                    product.PictureFilename = fileUpload.SaveAsImageFile(product.Name);
                
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                
                TempData["alertbox"] = $"Product '{product.Name}' created successfully.";
                return RedirectToAction("Manage");
            } 
            
            TempData["alertcard"] = "There are some validation errors. Please check and try again.";
            return View("Manage", await _db.Products.ToListAsync());			
        }

        // GET: Products/Edit/5
        // [Authorize(Roles = "staff")]
        public async Task<IActionResult> Edit(int id)
        {
            Product? product = await _db.Products.FindAsync(id);

            if (product == null) {
                TempData["alertbox"] = "Product does not exist.";
                return RedirectToAction("Manage");
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product updatedProduct, IFormFile? fileUpload)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(updatedProduct).State = EntityState.Modified;

                if (fileUpload != null) // Update picture
                    updatedProduct.PictureFilename = fileUpload.SaveAsImageFile(updatedProduct.Name);
                else // Retain the current picture
                    _db.Entry(updatedProduct).Property(x => x.PictureFilename).IsModified = false;

                await _db.SaveChangesAsync();

                TempData["alertbox"] = $"Product '{updatedProduct.Name}' updated successfully.";
                return RedirectToAction("Manage");
            }

            TempData["alertcard"] = "There are some validation errors. Please check and try again.";
            return View(updatedProduct);
        }

        // GET: Products/Delete/5
        // [Authorize(Roles = "staff")]
        public async Task<IActionResult> Delete(int id)
        {
            Product? product = await _db.Products.FindAsync(id);
            if (product != null) {
                string name = product.Name;
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                TempData["alertbox"] = $"Product '{name}' deleted successfully.";
            } else {
                TempData["alertbox"] = "Product not found";
            }

            return RedirectToAction("Manage");
        }
    }
}
