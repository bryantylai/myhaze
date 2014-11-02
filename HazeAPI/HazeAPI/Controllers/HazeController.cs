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
        private HazeServiceV2 hazeServiceV2;

        public HazeController()
        {
            this.hazeService = new HazeService();
            this.hazeServiceV2 = new HazeServiceV2();
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
            //return await this.hazeService.HazeDetailsById(hazeId, new Haze());
            return await this.hazeServiceV2.HazeDetailsById(hazeId, new Haze());
        }

        /// <summary>
        /// Get history of Haze by Haze Id
        /// </summary>
        /// <param name="hazeId">Haze Id</param>
        /// <returns>JSON response of History objects</returns>
        [Route("haze/history/{hazeId}")]
        public async Task<HazeWithHistory> GetHazeWithHistoryById(string hazeId)
        {
            HazeWithHistory hazeWithHistory = new HazeWithHistory();
            hazeWithHistory.Haze = new Haze();
            hazeWithHistory.Histories = new LinkedList<History>();
            return await this.hazeServiceV2.HazeHistoryById(hazeId, hazeWithHistory);
        }
    }
}
