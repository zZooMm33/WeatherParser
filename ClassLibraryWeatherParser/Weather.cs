using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibraryWeatherParser
{
    public class Weather
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string State { get; set; }

        public float Temperature { get; set; }

        public float WindSpeed { get; set; }

        public int ChancePrecipitation { get; set; }

        public int AirHumidity { get; set; }

        public City City { get; set; }

    }
}
