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
				Price = 7.50M
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

    }



}