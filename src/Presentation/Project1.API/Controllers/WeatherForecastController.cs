using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project1.API.ActionFilters.AuditlogFilters;
using Project1.Application.Products;
using Project1.Core.Products.Interfaces;

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
        private readonly IProductService _p;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IProductService p)
        {
            _logger = logger;
            _p = p;
        }

        [Authorize]
        [HttpGet(Name = "GetWeatherForecast")]
        [TypeFilter(typeof(AuditLoggingFilter))]
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

        [HttpGet("GetP")]
        public async Task<IActionResult> GetP([FromQuery] long id)
        {
            var product = await _p.GetProductAsync(id);

            return Ok(product);
        }
    }
}
