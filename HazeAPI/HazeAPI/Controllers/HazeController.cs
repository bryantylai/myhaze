using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using HazeAPI.Models;
using HazeAPI.Services;

namespace HazeAPI.Controllers
{
    [RoutePrefix("api/hazemy")]
    public class HazeController : ApiController
    {
        private HazeService hazeService;

        public HazeController()
        {
            this.hazeService = new HazeService();
        }

        [Route("fetch/{hazeId}/haze")]
        public Task<Haze> GetHaze(String hazeId)
        {
            return this.hazeService.HazeDetailsById(hazeId);
        }

        [Route("fetch/haze")]
        public Task<IEnumerable<Haze>> GetHaze()
        {
            return this.hazeService.ListHazeInDetails();
        }
        
        [Route("fetch/{weatherId}/weather")]
        public Task<Weather> GetWeather(string weatherId)
        {
            return this.hazeService.WeatherDetailsById(weatherId);
        }

        [Route("fetch/weather")]
        public Task<IEnumerable<Weather>> GetWeather()
        {
            return this.hazeService.ListWeatherInDetails();
        }
    }
}
