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

        [Route("{hazeId}/haze")]
        public Task<City> GetHaze(String hazeId)
        {
            return this.hazeService.CityDetailsById(hazeId, new City());
        }

        [Route("haze")]
        public Task<IEnumerable<State>> GetHaze()
        {
            return this.hazeService.ListCityInDetails();
        }
    }
}
