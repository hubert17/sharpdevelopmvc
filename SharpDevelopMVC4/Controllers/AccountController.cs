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
			return View();
		}
		
		[HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]		
		public ActionResult Login(string username, string password, bool rememberme = false)
		{
			if(UserAccount.Authenticate(username, password))
		    {
		    	var user = UserAccount.GetUserByUserName(username);
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
		    ModelState.AddModelError("", "Invalid username or password");
		    return View();
		}
		
		public ActionResult Logoff()
		{
		    FormsAuthentication.SignOut();
		    return RedirectToAction("Index", "Home");
		}
	}
}