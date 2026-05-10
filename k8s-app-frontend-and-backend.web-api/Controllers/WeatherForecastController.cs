using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using k8s_app_frontend_and_backend.web_api.Configuration;

namespace k8s_app_frontend_and_backend.web_api.Controllers
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
        private readonly WeatherApiConfig _config;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IOptions<WeatherApiConfig> configOptions)
        {
            _logger = logger;
            _config = configOptions.Value;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Weather forecast requested. API Version: {ApiVersion}", _config.ApiVersion);

            return Enumerable.Range(1, _config.MaxForecastDays).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(_config.MinTemperatureC, _config.MaxTemperatureC),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// Returns current configuration (useful for debugging in K8s)
        /// </summary>
        [HttpGet("config", Name = "GetConfig")]
        public IActionResult GetConfig()
        {
            return Ok(new
            {
                _config.MaxForecastDays,
                _config.MinTemperatureC,
                _config.MaxTemperatureC,
                _config.EnableCaching,
                _config.CacheDurationSeconds,
                _config.ApiVersion,
                Environment = _logger.ToString()
            });
        }
    }
}
