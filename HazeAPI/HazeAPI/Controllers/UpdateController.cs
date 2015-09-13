using HazeAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace HazeAPI.Controllers
{
    [RoutePrefix("update")]
    public class UpdateController : ApiController
    {
        [Route("")]
        [HttpGet]
        public async Task<string> Update()
        {
            try
            {
                UpdateService updateService = new UpdateService();
                bool result = await updateService.Update();
                return result.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
