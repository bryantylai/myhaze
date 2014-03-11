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

        /// <summary>
        /// Get Haze by Haze Id
        /// </summary>
        /// <param name="hazeId">Haze Id</param>
        /// <returns>JSON response of City object</returns>
        [Route("{hazeId}")]
        [Obsolete]
        public Task<City> GetHaze(String hazeId)
        {
            return this.hazeService.CityDetailsById(hazeId, new City());
        }

        /// <summary>
        /// Get Haze by Haze Id
        /// </summary>
        /// <param name="hazeId">Haze Id</param>
        /// <returns>JSON response of Haze object</returns>
        [Route("haze/{hazeId}")]
        public async Task<Haze> GetHazeById(string hazeId)
        {
            return await this.hazeService.HazeDetailsById(hazeId, new Haze());
        }

        /// <summary>
        /// Get history of Haze by Haze Id
        /// </summary>
        /// <param name="hazeId">Haze Id</param>
        /// <returns>JSON response of History objects</returns>
        [Route("haze/{hazeId}/history")]
        public async Task<IEnumerable<History>> GetHazeWithHistoryById(string hazeId)
        {
            return await this.hazeService.HazeHistoryById(hazeId, new LinkedList<History>());
        }
    }
}
