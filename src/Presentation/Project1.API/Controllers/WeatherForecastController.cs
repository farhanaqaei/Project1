using Microsoft.AspNetCore.Mvc;
using Project1.Core.ProductAggregate.Interfaces;
using Project1.Core.ProductAggregate.Interfaces.DTOs;

namespace Project1.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IProductService _ps;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IProductService ps)
        {
            _logger = logger;
            _ps = ps;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<long> gggggg(AddProductDTO inp)
        {
            var s = await _ps.AddProduct(inp);
            return s;
        }
    }
}
