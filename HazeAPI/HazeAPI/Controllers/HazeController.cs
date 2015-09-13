using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using HazeAPI.Contracts;
using HazeAPI.Services.Obsolete;

namespace HazeAPI.Controllers
{
    [RoutePrefix("api/hazemy")]
    public class HazeController : ApiController
    {
        private HazeService hazeService;
        private Services.HazeService hazeServiceV3;

        public HazeController()
        {
            this.hazeService = new HazeService();
            this.hazeServiceV3 = new Services.HazeService();
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
        public Haze GetHazeById(string hazeId)
        {
            return this.hazeServiceV3.GetSingle(hazeId);
        }

        /// <summary>
        /// Get history of Haze by Haze Id
        /// </summary>
        /// <param name="hazeId">Haze Id</param>
        /// <returns>JSON response of History objects</returns>
        [Route("haze/history/{hazeId}")]
        public HazeWithHistory GetHazeWithHistoryById(string hazeId)
        {
            return this.hazeServiceV3.Get(hazeId);
        }

        /// <summary>
        /// Get history of Haze by Haze Id
        /// </summary>
        /// <param name="hazeId">Haze Id</param>
        /// <returns>JSON response of History objects</returns>
        [Route("v2/haze/history/{hazeId}")]
        public HazeWithHistoryContainer GetHazeWithHistoryContainerById(string hazeId)
        {
            HazeWithHistoryContainer hazeWithHistoryContainer = new HazeWithHistoryContainer();
            try
            {
                hazeWithHistoryContainer.HazeWithHistory = this.hazeServiceV3.Get(hazeId);
            }
            catch (Exception ex)
            {
                ExceptionLite exceptionLite = new ExceptionLite();
                exceptionLite.Name = ex.GetType().Name;
                exceptionLite.Message = ex.Message;
                exceptionLite.StackTrace = ex.StackTrace;

                hazeWithHistoryContainer.Exception = exceptionLite;
            }

            return hazeWithHistoryContainer;
        }
    }
}
