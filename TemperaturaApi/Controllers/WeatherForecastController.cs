using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperaturaApi.Controllers.Repositories;

namespace TemperaturaApi.Controllers
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
        private readonly IWeatherRepository _repository;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WeatherForecast entity)
        {
            var result = await _repository.AddAsync(entity);
            if (result == null)
            {
                _logger.LogError("Entity already exists with id '{Id}'", entity.Id);
                return Conflict();
            }
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }
    }
}
