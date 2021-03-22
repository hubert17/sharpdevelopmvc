using System;
using System.Web.Mvc;

namespace SharpDevelopMVC4.Controllers
{
	public class SongController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
		
		public ActionResult GetSong()
		{
			return View();
		}

	}
}