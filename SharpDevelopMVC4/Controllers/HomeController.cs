using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using Dapper;
using Newtonsoft.Json;

namespace SharpDevelopMVC4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
        	string mdb = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AppDomain.CurrentDomain.GetData("DataDirectory") + @"\MyAccessDb.mdb";
        	
			//Generate Class from Table
//			using (var connection = new OleDbConnection(mdb))
//			{	
//				ViewBag.Data = connection.GenerateClass("select * from Songs");
//			}
		
//        	var contact = new 
//        	{
//        		FullName = "Hewbhurt Gabon",
//        		Email = "gabs@gmail.com",
//        		BirthDate = "1981/09/17"
//        	};

            using (var conn = new OleDbConnection(mdb))
            {
                // conn.Execute( "INSERT INTO contacts(FullName, Email, BirthDate) "
                // + "VALUES (@FullName, @Email, @BirthDAte)", contact);
                var contactList = conn.Query("Select Id, FullName, Email, BirthDate from contacts").ToList();
                ViewBag.Data = JsonConvert.SerializeObject(contactList);
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