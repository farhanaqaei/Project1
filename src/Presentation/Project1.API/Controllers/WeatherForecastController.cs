using Microsoft.AspNetCore.Mvc;
using Project1.API.ActionFilters;

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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [TypeFilter(typeof(AuditLoggingFilter))]
        public IEnumerable<WeatherForecast> Get()
        {
            throw new NotImplementedException();
        }
    }
}
