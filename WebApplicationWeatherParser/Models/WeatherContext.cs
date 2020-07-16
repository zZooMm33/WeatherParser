using ClassLibraryWeatherParser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationWeatherParser.Models
{
    public class WeatherContext : DbContext
    {
        public DbSet<Weather> Weathers { get; set; }

        public DbSet<City> Cities { get; set; }

        public WeatherContext(DbContextOptions<WeatherContext> dbContextOptions) : base (dbContextOptions)
        {
            Database.EnsureCreated();
        }
    }
}
