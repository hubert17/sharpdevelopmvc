using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SharpDevelopMVC4.Controllers
{
	/// <summary>
	/// Description of AccountController.
	/// </summary>
	public class AccountController : Controller
	{
		public ActionResult Login()
		{
			// Logout account
			FormsAuthentication.SignOut();
			// Then return login form
			return View();
		}
		
		
        [AllowAnonymous]
		[HttpPost]	        
        [ValidateAntiForgeryToken]		
		public ActionResult Login(string username, string password, bool rememberme = false)
		{		
			var user = UserAccountCSV.Authenticate(username, password);
			if(user != null) // If not null then it's a valid login
		    {
				var authTicket = new FormsAuthenticationTicket(
				    1,                             	// version
				    user.UserName,               	// user name
				    DateTime.Now,                  	// created
				    DateTime.Now.AddMinutes(20),   	// expires
				    rememberme,                    	// persistent?
		    		user.Roles              		// can be used to store roles
			    );
				
				string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
				
				var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
				System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
				
				Session["user"] = user.UserName;
				
				return Redirect(FormsAuthentication.GetRedirectUrl(user.UserName, rememberme)); // auth succeed
				
		    }
		    
		    // invalid username or password
		    ModelState.AddModelError("invalidLogin", "Invalid username or password");
		    return View();
		}
		
		public ActionResult Logoff()
		{
		    FormsAuthentication.SignOut();
		    return RedirectToAction("Index", "Home");
		}
		
		public ActionResult Register(string username, string password, string role = "")
		{
			if(role.ToLower() == "admin") role = "user"; // Prevent unauthorized creation of admin account			
			var result = UserAccountCSV.Create(username, password, role);
			return Content(result.UserName);
		}
		
		[Authorize(Roles="admin")]
		public ActionResult ManageUsers()
		{		
			return View();
		}
		
		[Authorize(Roles="admin")]
		[Route("/account.csv")]
		public ActionResult GetUsersCSV()
		{
			var file = UserAccountCSV.GetCsvFile();
			return File(file, "text/csv");	
		}
	}
}