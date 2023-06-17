using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPNETWebApp45.Models;

namespace ASPNETWebApp45.Controllers
{
    public class WeatherForecastController : Controller
    {       
        // GET: WeatherForecast
        [HttpGet]
        public ActionResult Get(int maxItem = 5)
        {
            var forecasts = WeatherForecast.SampleForecasts(maxItem).ToList();

            return Json(forecasts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexMvc(int maxItem = 5)
        {
            var forecasts = WeatherForecast.SampleForecasts(maxItem).ToList();

            return View(forecasts);
        }

        public ActionResult GetForecastsPartialView(int maxItem = 5)
        {
            var forecasts = WeatherForecast.SampleForecasts(maxItem).ToList();

            return PartialView("_ForecastsPartialView", forecasts);
        }
    }
}