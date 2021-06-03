using System;
using System.Linq;
using System.Web.Mvc;

namespace SharpDevelopMVC4.Controllers
{
	public class SongController : Controller
	{
		Gabs2021DbContext _db = new Gabs2021DbContext();
		
		// Read or List all the songs 
		public ActionResult Index()
		{
			var songs = _db.Songs.ToList();
		
			return View(songs);
		}
		
		// Read a single song
		public ActionResult GetById(int Id)
		{
			return View();
		}
			
		//  Display a page for adding a new song
		public ActionResult Create()
		{
			return View();
		}
		
		// Add a new song
		[HttpPost]
		public ActionResult Create(Song newSong) // Model binding
		{
			_db.Songs.Add(newSong);
			_db.SaveChanges(); // Push the new item to the db
								
			return RedirectToAction("Index");
		}
		
		// Display a page for editing a song by its Id
		// /song/edit/Id
		public ActionResult Edit(int Id)
		{
			var song = _db.Songs.Find(Id);			
			return View(song);
		}
		
		// Update a song
		[HttpPost]
		public ActionResult Edit(Song updatedSong)
		{
	        _db.Entry(updatedSong).State = System.Data.Entity.EntityState.Modified;
	        _db.SaveChanges();
	        
	        return RedirectToAction("Index");
		}
		
		// Delete a song
		public ActionResult Delete(int Id)
		{
			var songToBeDeleted = _db.Songs.Find(Id);
			_db.Songs.Remove(songToBeDeleted);
			_db.SaveChanges();
						
			return RedirectToAction("Index");
		}
		

	}
}