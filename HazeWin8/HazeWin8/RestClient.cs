﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Popups;

namespace HazeWin8
{
    public class RestClient
    {
        private const string API_URL = "http://localhost:44956/api/hazemy/haze/";
        //private const string API_URL = "http://myhaze-api.azurewebsites.net/api/hazemy/haze/";
        private HttpClient client = new HttpClient();
        public async void Get<T>(string hazeId, Action<T> callback)
        {
            bool internalServerErr = false;
            try
            {
                var result = await client.GetStringAsync(API_URL + hazeId);
                callback(JsonConvert.DeserializeObject<T>(result));
            }
            catch (Exception)
            {
                internalServerErr = true;
                callback(default(T));
            }

            if (internalServerErr)
            {
                MessageDialog errorMessage = new MessageDialog("There has been an error while retrieving data. Please try again later.");
                await errorMessage.ShowAsync();
            }
        }
    }

}
