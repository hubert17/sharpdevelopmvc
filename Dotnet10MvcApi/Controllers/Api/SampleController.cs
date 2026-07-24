using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet10MvcApi.Data;
using Dotnet10MvcApi.Helpers;
using Dotnet10MvcApi.Models;
using Dotnet10MvcApi.Models.Dtos;
using Dotnet10MvcApi.Models.Entities;
using Dotnet10MvcApi.Services;

namespace Dotnet10MvcApi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SampleController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Wake up database check
        [HttpGet("WakeUpAzureDb")]
        public IActionResult WakeUpAzureDb()
        {
            DateTime dbServerDate;
            try
            {
                // In Access Jet and PostgreSQL, SELECT NOW() works.
                dbServerDate = _db.Database.SqlQueryRaw<DateTime>("SELECT NOW()").First();
            }
            catch
            {
                // Fallback to local server date
                dbServerDate = DateTime.Now;
            }
            
            return Ok(new { awaken = true, dbServerDate });
        }

        // GET: WeatherForecast
        [HttpGet("getWeather")]
        public IActionResult GetWeather(int maxItem = 5)
        {
            var forecasts = WeatherForecast.SampleForecasts(maxItem);
            return Ok(forecasts);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("getproduct")]
        public IActionResult GetProduct()
        {
            var product = new
            {
                Id = 1,
                Name = "Ariel",
                Price = 7.50M
            };
            
            return Ok(product);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            if (files != null && files.Count > 0)
            {
                var docfiles = new List<string>();
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var filePath = Path.Combine(uploadPath, file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        docfiles.Add(file.FileName);
                    }
                }
                return Ok(new { docfiles });
            }

            return BadRequest("No files uploaded.");
        }

        [HttpPost("uploadphoto")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return Ok(filePath);
            }

            return BadRequest("No file uploaded.");
        }

        [HttpPost("sendmail")]
        public IActionResult SendEmail(string EmailTo, string Subject, string Message)
        {
            var success = EmailService.SendEmail(EmailTo, Subject, Message);
            if (success)
                return Ok("Successfully sent.");
            
            return BadRequest("Sending failed.");
        }
    }
}
