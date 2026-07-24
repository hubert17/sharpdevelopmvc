using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dotnet10MvcApi.Models;

namespace Dotnet10MvcApi.Controllers
{
    public class WeatherForecastController : Controller
    {       
        [HttpGet]
        public IActionResult Get(int maxItem = 5)
        {
            var forecasts = WeatherForecast.SampleForecasts(maxItem).ToList();
            return Json(forecasts);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexMvc(int maxItem = 5)
        {
            var forecasts = WeatherForecast.SampleForecasts(maxItem).ToList();
            return View(forecasts);
        }

        public IActionResult _GetForecasts(int maxItem = 5)
        {
            var forecasts = WeatherForecast.SampleForecasts(maxItem).ToList();
            return PartialView("_ForecastsPartialView", forecasts);
        }
    }
}
