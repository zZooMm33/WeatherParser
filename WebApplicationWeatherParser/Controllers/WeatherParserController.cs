using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryWeatherParser;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationWeatherParser.Models;
using WebApplicationWeatherParser.Utils;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationWeatherParser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherParserController : ControllerBase
    {
        const string _urlWeather = "https://world-weather.ru/archive/russia/";

        WeatherContext _weatherContext;

        public WeatherParserController(WeatherContext weatherContext)
        {
            _weatherContext = weatherContext;
        }

        [HttpGet]
        public int Get(string name, string date)
        {
            DateTime dateTime = Convert.ToDateTime(date);
            string cityUrl = "https:";
            HtmlDocument htmlSnippet = new HtmlDocument();
            Weather newWeather = new Weather();

            Weather oldWeather = _weatherContext.Weathers.Where(weather => weather.Date == dateTime && weather.City.Name == name).Include(weather => weather.City).ToList().FirstOrDefault();
            if (oldWeather != null)
            {
                _weatherContext.Weathers.Remove(oldWeather);
                _weatherContext.SaveChanges();
            }

            City city = _weatherContext.Cities.Where(city => city.Name == name).ToList().FirstOrDefault();
            if (city == null)
                city = new City() { Name = name };

            htmlSnippet.LoadHtml(GetHtmlPage.Get(_urlWeather));

            foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//a[@href]"))
            {
                if (link.InnerText == name)
                {
                    cityUrl += link.Attributes["href"].Value;
                    break;
                }                
            }

            if (cityUrl != "")
            {                
                HtmlNode htmlPage = null;

                if ((dateTime - DateTime.Now.Date).TotalDays < 0)
                {
                    cityUrl += dateTime.ToString("dd") + "-" + dateTime.ToString("MMMM", CultureInfo.GetCultureInfo("en-us")).ToLower() + "/";
                    cityUrl = cityUrl.Replace("archive", "pogoda");
                    htmlSnippet.LoadHtml(GetHtmlPage.Get(cityUrl));
                    
                    if (dateTime.Month < DateTime.Now.Month || (dateTime.Month == DateTime.Now.Month && dateTime.Day < DateTime.Now.Day + 10))
                    {
                        int isShort = 0;
                        if (htmlSnippet.DocumentNode.SelectSingleNode("//table[@class='weather-today short']") != null) isShort = 1;

                        if (DateTime.Now.Year == dateTime.Year)
                        {
                            htmlPage = htmlSnippet.DocumentNode.SelectNodes("//table[@class='weather-today']")[1].SelectSingleNode(".//tr[@class='day']");
                            if (htmlPage == null) htmlPage = htmlSnippet.DocumentNode.SelectSingleNode("//table[@class='weather-today short']").SelectSingleNode(".//tr[@class='day']");
                        }
                        else if (DateTime.Now.Year - 1 == dateTime.Year) htmlPage = htmlSnippet.DocumentNode.SelectNodes("//table[@class='weather-today']").ToList()[3 - isShort].SelectSingleNode(".//tr[@class='day']");
                        else if (DateTime.Now.Year - 2 == dateTime.Year) htmlPage = htmlSnippet.DocumentNode.SelectNodes("//table[@class='weather-today']").ToList()[5 - isShort].SelectSingleNode(".//tr[@class='day']");
                        else return 0;
                    }
                    else
                    {
                        if (DateTime.Now.Year - 1 == dateTime.Year) htmlPage = htmlSnippet.DocumentNode.SelectNodes("//table[@class='weather-today']")[1].SelectSingleNode(".//tr[@class='day']");
                        else if (DateTime.Now.Year - 2 == dateTime.Year) htmlPage = htmlSnippet.DocumentNode.SelectNodes("//table[@class='weather-today']")[3].SelectSingleNode(".//tr[@class='day']");
                        else return 0;
                    }

                    newWeather.WindSpeed = float.Parse(htmlPage.SelectNodes(".//td[@class='weather-wind']").FirstOrDefault().InnerText.Replace(" ", "").Replace(".", ","));
                    newWeather.AirHumidity = Convert.ToInt32(htmlPage.SelectNodes(".//td[@class='weather-humidity']").FirstOrDefault().InnerText.Replace("%", ""));
                }
                else
                {
                    cityUrl += "14days/";
                    cityUrl = cityUrl.Replace("archive", "pogoda");
                    htmlSnippet.LoadHtml(GetHtmlPage.Get(cityUrl));

                    foreach (HtmlNode htmlWeather in htmlSnippet.DocumentNode.SelectNodes("//div[@class='weather-short']"))
                    {
                        if (htmlWeather.SelectNodes(".//div").FirstOrDefault().InnerText.IndexOf(dateTime.ToString("dd")) != -1)
                        {
                            htmlPage = htmlWeather.SelectNodes(".//tr[@class='day fourteen-d']").FirstOrDefault();
                            break;
                        }
                    }

                    newWeather.ChancePrecipitation = Convert.ToInt32(htmlPage.SelectNodes(".//td[@class='weather-probability']").FirstOrDefault().InnerText.Replace("%", ""));
                    newWeather.WindSpeed = float.Parse(htmlPage.SelectNodes(".//td[@class='weather-wind ']").FirstOrDefault().InnerText.Replace(" ", "").Replace(".", ","));
                    newWeather.AirHumidity = Convert.ToInt32(htmlPage.SelectNodes(".//td[@class='weather-humidity ']").FirstOrDefault().InnerText.Replace("%", ""));
                }

                newWeather.State = htmlPage.SelectNodes(".//td[@class='weather-temperature']").FirstOrDefault().SelectNodes(".//div").FirstOrDefault().Attributes["title"].Value;
                newWeather.Temperature = float.Parse(htmlPage.SelectNodes(".//td[@class='weather-feeling']").FirstOrDefault().InnerText
                    .Replace("+", "")
                    .Replace("°", ""));                
                newWeather.Date = dateTime;
                newWeather.City = city;

                _weatherContext.Weathers.Add(newWeather);
                _weatherContext.SaveChanges();

                return 1;
            }
            else return 0;
        }
    }
}