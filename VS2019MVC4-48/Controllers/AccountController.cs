using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ASPNETWebApp48.Controllers
{
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
			var valid = UserAccount.Authenticate(username, password);
			if (valid)
			{
				var user = UserAccount.GetCurrentUser();
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

				if (username.ToLower() == UserAccount.DEFAULT_ADMIN_LOGIN.ToLower() && password == UserAccount.DEFAULT_ADMIN_LOGIN.ToLower())
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
			Session.Abandon();
			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		public ActionResult ChangePassword()
        {			
			return View();
        }

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult ChangePassword(string currentPassword, string newPassword)
		{
			bool success = UserAccount.ChangePassword(User.Identity.Name, currentPassword, newPassword);
			if (success)
				TempData["alertbox"] = "Password changed successfully.";
			else
				TempData["alertbox"] = "Failed to change password.";

			return RedirectToAction("Logoff");
		}

		public ActionResult Register()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(string username, string password, string role = "")
		{
			if(role.ToLower().Contains(UserAccount.DEFAULT_ADMIN_ROLENAME)) role = "user"; // Prevent unauthorized creation of admin account			
			var result = UserAccount.Create(username, password, role);
			if (result != null)
			{
				TempData["alert"] = String.Format("Account successfully created. Welcome {0}!", username);
				return RedirectToAction("Login");
			}
			else
			{
				TempData["alertbox"] = "There was an issue creating your account.";
				return RedirectToAction("Index", "Home");
			}
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
				UserAccount.SetUserActivation(username, false);
				TempData["alertbox"] = username + " is now deactivated.";
			}

			return RedirectToAction("Index", "Home");
		}

	}
}