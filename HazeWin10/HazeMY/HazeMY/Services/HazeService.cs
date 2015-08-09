using HazeMY.Models;
using NetworkClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazeMY.Services
{
    public interface IHazeService
    {
        Task<HazeWithHistoryContainer> GetByLocationId(string locationId);
    }

    public class HazeService : IHazeService
    {
        private RestClient _restClient;
        public RestClient RestClient
        {
            get
            {
                return _restClient ?? (_restClient = new RestClient("Haze MY"));
            }
        }

        //private readonly string GetByLocationIdUri = "http://localhost:44956/api/hazemy/v2/haze/history/{0}";
        private readonly string GetByLocationIdUri = "http://myhaze-api.azurewebsites.net/api/hazemy/v2/haze/history/{0}";

        public Task<HazeWithHistoryContainer> GetByLocationId(string locationId)
        {
            string query = string.Format(GetByLocationIdUri, locationId);
            return RestClient.GetAsync<HazeWithHistoryContainer>(query);
        }
    }
}
