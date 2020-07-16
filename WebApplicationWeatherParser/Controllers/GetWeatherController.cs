using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryWeatherParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationWeatherParser.Models;

namespace WebApplicationWeatherParser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetWeatherController : ControllerBase
    {
        WeatherContext _weatherContext;

        public GetWeatherController(WeatherContext weatherContext)
        {
            _weatherContext = weatherContext;
        }

        [HttpGet]
        public Weather Get(string name, string date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            return _weatherContext.Weathers.Where(weather => weather.Date == dateTime && weather.City.Name == name).Include(weather => weather.City).ToList().FirstOrDefault();
        }
    }
}