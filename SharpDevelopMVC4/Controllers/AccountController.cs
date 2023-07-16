using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ASPNETWebApp45.Controllers
{
	/// <summary>
	/// Description of AccountController.
	/// </summary>
	public class AccountController : Controller
	{
		[AllowAnonymous]
		public ActionResult Login(string returnUrl = "/")
		{
			// Logout account
			FormsAuthentication.SignOut();
			// Then return login form
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}		
		
        [AllowAnonymous]
		[HttpPost]	        
        [ValidateAntiForgeryToken]		
		public ActionResult Login(string username, string password, bool rememberme = false, string returnUrl = "/")
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
				
				if (username.ToLower() == UserAccountCSV.DEFAULT_ADMIN_LOGIN.ToLower() && password == UserAccountCSV.DEFAULT_ADMIN_LOGIN.ToLower())
					return RedirectToAction("ChangePassword");
				else
					return Redirect(FormsAuthentication.GetRedirectUrl(user.UserName, rememberme)); // auth succeed				
		    }
		    
		    // invalid username or password
		     TempData["alert"] = "Invalid username or password";
			return RedirectToAction("Login", new { ReturnUrl = returnUrl });
		}
		
		public ActionResult Logoff()
		{
		    FormsAuthentication.SignOut();
		    return RedirectToAction("Index", "Home");
		}

		[Authorize]
		public ActionResult ChangePassword()
        {			
			return View();
        }

		[HttpPost]
		[Authorize]
		public ActionResult ChangePassword(string currentPassword, string newPassword)
		{
			bool success = UserAccountCSV.ChangePassword(User.Identity.Name, currentPassword, newPassword);
			if (success)
				TempData["alertbox"] = "Password changed successfully.";
			else
				TempData["alertbox"] = "Failed to change password.";

			return RedirectToAction("Logoff");
		}

		// Account/Register?username=user01&password=pass123
		[AllowAnonymous]
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
		
		// /Account/Deactivate?username=user01
		[Authorize(Roles = "admin")]
		public ActionResult Deactivate(string username)
		{
			if (username == "admin")
				TempData["alert"] = "Admin account cannot be deactivated.";
			else
			{
				UserAccountCSV.SetUserActivation(username, false);
				TempData["alertbox"] = username + " is now deactivated.";
			}

			return RedirectToAction("Index", "Home");
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