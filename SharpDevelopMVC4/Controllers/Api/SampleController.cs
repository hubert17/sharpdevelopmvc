using CsvHelper;
using Hangfire;
using JWTAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using ASPNETWebApp45.Models;
using RestSharp;

namespace ASPNETWebApp45.Controllers.Api
{
    /// <summary>
    /// Description of SampleController.
    /// </summary>
    public class SampleController : ApiController
	{
		// GET: WeatherForecast
		[HttpGet]
		[Route("api/sample/getWeather")]
		public IHttpActionResult Get(int maxItem = 5)
		{
            var forecasts = WeatherForecast.SampleForecasts(maxItem);

			return Ok(forecasts);
		}

		[ApiAuthorize]
		[HttpGet]
		[Route("api/sample/getproduct")]
		public IHttpActionResult GetProduct()
		{
			var product = new
			{
				Id = 1,
				Name = "Ariel",
				Price = 7.50M,
				Username = User.Identity.Name
			};
        	
			return Ok(product);
		}

        [HttpPost]
        [FileUpload.SwaggerForm()]
        [Route("api/sample/upload")]
        public IHttpActionResult UploadFile()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    docfiles.Add(System.IO.Path.GetFileName(filePath));
                }
                return Ok(new { docfiles });
            }

            return BadRequest();
        }

        [HttpPost]
        [FileUpload.SwaggerForm("Image File", "Upload image for multipart/form-data")]
        [Route("api/sample/uploadphoto")]
        public IHttpActionResult UploadImage()
        {
            var postedFile = HttpContext.Current.Request.Files[0];
            var filePath = postedFile.SaveToFolder();
            return Ok(filePath);
        }



        [HttpPost]
        [Route("api/sample/sendmail")]
        public IHttpActionResult SendEmail(string EmailTo, string Subject, string Message)
        {
            var success = EmailService.SendEmail(EmailTo, Subject, Message);
            if (success)
                return Ok("Successfully sent.");
            else
                return BadRequest("Sending failed.");
        }
        
        
        [AllowAnonymous]
        [HttpGet]
        [Route("api/sample/bingphotos")]
        public IHttpActionResult GetBingPhotos()
        {
        	var csvFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "BingPhotos.csv");
        	List<BingImage> bingImages;
	        if (System.IO.File.Exists(csvFile))
	        {
	            using (var reader = new System.IO.StreamReader(csvFile))
	            using (var csv = new CsvReader(reader))
	            {
	                bingImages = csv.GetRecords<BingImage>().ToList();
	                return Ok(bingImages);
	            }
	        }
	        
	        return BadRequest("Bing photos unavailable.");
        } 

        [AllowAnonymous]
        [HttpGet]
        [Route("api/sample/news")]
        public IHttpActionResult GetNews(string apiKey = "", string category = "", int pageSize = 20, int page = 1)
        {
        	if(string.IsNullOrEmpty(apiKey)) apiKey = "1db81ac8b6b64bdd9dfb5bed2647f495";
        	var cacheName = ("newapi" + category + page) + pageSize;
        	if(HttpRuntime.Cache[cacheName] == null)
        	{
	           	if(!string.IsNullOrWhiteSpace(category))
	        		category = "&category=" + category;
	           	
				IRestClient client = new RestClient("https://newsapi.org");  
				var request = new RestRequest("/v2/top-headlines?country=ph" + category + "&pageSize=" + pageSize + "&page=" + page + "&apiKey=" + apiKey, Method.GET);
				var response = client.Execute<NewsAPI>(request);
				var data = response.Data;
				if(data.articles.Any())
				{
					HttpRuntime.Cache[cacheName] = data;    
					RecurringJob.AddOrUpdate("removeCache" + cacheName, () => HangfireJobsApi45.RemoveNewsCache(cacheName), "* */2 * * *");	
				}
        	}

        	return Ok(HttpRuntime.Cache[cacheName]);
        }

   

        [AllowAnonymous]
        [HttpGet]
        [Route("api/sample/hangfire")]
        public IHttpActionResult  MonitorHangfire()
        {
			var api = JobStorage.Current.GetMonitoringApi();
			var ScheduledJob = api.ScheduledJobs(0,100);
			var	HourlySucceededJobs = api.HourlySucceededJobs();
			var HourlyFailedJobs = api.HourlyFailedJobs();
			var SucceededJobs = api.SucceededJobs(0,100);
			var FailedJobs = api.FailedJobs(0,100);				
			var stats = new {
				Enqueued = api.GetStatistics().Enqueued,
				Recurring = api.GetStatistics().Recurring,
				Succeeded = api.GetStatistics().Succeeded,
				Failed = api.GetStatistics().Failed
			};
			
			return Ok(new { stats, ScheduledJob, HourlySucceededJobs, HourlyFailedJobs, SucceededJobs, FailedJobs });
        } 

    }



}