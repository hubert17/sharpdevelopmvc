/*
 * Created by SharpDevelop.
 * User: Gabs
 * Date: 19/07/2019
 * Time: 2:03 am
 */
 
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Hangfire;
using Newtonsoft.Json.Serialization;
using Hangfire.MemoryStorage;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Linq;

public class ApplicationPreload : System.Web.Hosting.IProcessHostPreloadClient
{
    public void Preload(string[] parameters)
    {
        HangfireBootstrapper.Instance.Start();
    }
}


namespace ASPNETWebApp45
{
	public class MvcApplication : HttpApplication
	{
		const string API_Route_Prefix = "api";

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.Ignore("{resource}.axd/{*pathInfo}");

			routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: API_Route_Prefix + "/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional });

			routes.MapRoute(
				"Default",
				"{controller}/{action}/{id}",
				new {
					controller = "Home",
					action = "Index",
					id = UrlParameter.Optional,
				},
				new string[] { "ASPNETWebApp45.Controllers" });
		}

		protected void Application_Start()
		{     
			SimpleLogger.Init();        	
			AutoMapperConfig.Initialize();

			var config = System.Web.Http.GlobalConfiguration.Configuration;

			config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

			config.MapHttpAttributeRoutes();

			RegisterRoutes(RouteTable.Routes);
			
			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

			config.Formatters.Remove(config.Formatters.XmlFormatter);
			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
			config.EnsureInitialized();
			
		    ViewEngines.Engines.Clear();  
    		ViewEngines.Engines.Add(new RazorViewEngine());  

			// Configure Hangfire www.hangfire.io            
			HangfireBootstrapper.Instance.Start();
			
            RecurringJob.AddOrUpdate("get-bing-photos", () => HangfireJobsApi45.BingPhotos(), Cron.Daily);	

			Pinger.KeepAliveHangfire("https://api45gabs.azurewebsites.net"); // KeepAliveHangfire("https://mysite.com")		            
		}
        
        protected void Application_End(object sender, EventArgs e)
        {
            HangfireBootstrapper.Instance.Stop();
        }   
        
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
		{
			HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
			if (authCookie == null || authCookie.Value == "")
				return;

			var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

			var formsIdentity = new FormsIdentity(authTicket);

			var claimsIdentity = new ClaimsIdentity(formsIdentity);

			var roles = authTicket.UserData.Split(',');

			foreach (var role in roles)
			{
				claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
			}

			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			HttpContext.Current.User = claimsPrincipal;
		}

        #region Session
        // Avoid Session at all cost!!!
        public override void Init()
        {
            this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
			HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
        #endregion
        
		#region KeepAlive
		public static class Pinger
		{
			public static void KeepAliveHangfire(string siteUrl = null, int minuteInterval = 5)
			{
				if (!string.IsNullOrEmpty(siteUrl))
					Hangfire.RecurringJob.AddOrUpdate("keep-alive", () => Pinger.Ping(siteUrl + "/pinger"), string.Format("*/{0} * * * *", minuteInterval));
			}
			
			static readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
			public static void Ping(string url)
			{
				client.GetAsync(url);
			}
		}

		#endregion
    }
}
