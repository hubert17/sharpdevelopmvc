using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet10MvcApi.Models
{
    public class WeatherForecast
    {
        public string Date { get; set; } = string.Empty;

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; } = string.Empty;

        public static IEnumerable<WeatherForecast> SampleForecasts(int maxItem = 5)
        {
            string[] Summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var random = new Random();
            return Enumerable.Range(1, maxItem).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index).Date.ToString("dd/MM/yyyy"),
                TemperatureC = random.Next(-20, 55),
                Summary = Summaries[random.Next(Summaries.Length)]
            });
        }
    }
}
