using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HazeAPI.Models;

namespace HazeAPI.Services
{
    public class HazeService
    {
        internal async Task<Haze> HazeDetailsById(string hazeId)
        {
            return new Haze();
        }

        internal async Task<IEnumerable<Haze>> ListHazeInDetails()
        {
            return new HashSet<Haze>();
        }

        internal async Task<Weather> WeatherDetailsById(string weatherId)
        {
            return new Weather();
        }

        internal async Task<IEnumerable<Weather>> ListWeatherInDetails()
        {
            return new HashSet<Weather>();
        }
    }
}