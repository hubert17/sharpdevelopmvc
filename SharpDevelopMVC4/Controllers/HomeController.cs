using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using Dapper;

namespace SharpDevelopMVC4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
        	string mdb = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AppDomain.CurrentDomain.GetData("DataDirectory") + @"\MyAccessDb.mdb";
        	var contact = new 
        	{
        		FullName = "Hewbhurt Gabon",
        		Email = "gabs@gmail.com",
        		BirthDate = "1981/09/17"
        	};
        	
        	using (var conn = new OleDbConnection(mdb))
        	{
//        		conn.Execute( "INSERT INTO contacts(FullName, Email, BirthDate) "
//	            	+ "VALUES (@FullName, @Email, @BirthDAte)", contact);
        		var contactList = conn.Query("Select Id, FullName, Email, BirthDate from contacts").ToList();
				ViewBag.Data = Newtonsoft.Json.JsonConvert.SerializeObject(contactList);
        	}
        	
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}