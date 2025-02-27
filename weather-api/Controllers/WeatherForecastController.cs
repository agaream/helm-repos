using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace weather_api.Controllers
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

        private string varHost = "APPHOST";
        private string varEnv = "APPENVIRONMENT";
        private string whereiam = "UNKNOWN";

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //TODO: Generar un controlador específico para obtener información del entorno y host 
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            string? valueHost = Environment.GetEnvironmentVariable(varHost);
            string? valueEnv = Environment.GetEnvironmentVariable(varEnv);
            if (varHost != null && varEnv != null)
            {
                whereiam = string.Format("Environment:{0} - Host: {1}", valueEnv, valueHost);
            }
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                LocalInfo = whereiam,
            })
            .ToArray();
        }
    }
}