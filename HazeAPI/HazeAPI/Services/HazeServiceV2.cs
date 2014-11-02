using HazeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace HazeAPI.Services
{
    public class HazeServiceV2
    {
        private HttpClient client;
        public HazeServiceV2()
        {
            this.client = new HttpClient();
        }

        internal Task<Haze> HazeDetailsById(string hazeId, Haze haze)
        {
            return null;
        }

        internal Task<HazeWithHistory> HazeHistoryById(string hazeId, HazeWithHistory hazeWithHistory)
        {
            throw new NotImplementedException();
        }
    }
}