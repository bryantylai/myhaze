using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace HazeWP
{
    public class RestClient
    {
        private WebClient client = new WebClient();
        public void Get<T>(string url, Action<T> callback)
        {
            client.DownloadStringAsync(new Uri(url, UriKind.Absolute));
            client.DownloadStringCompleted += (sender, eventArgs) =>
            {
                bool internalServerErr = false;
                try
                {
                    var deserializedObject = JsonConvert.DeserializeObject<T>(eventArgs.Result);
                    callback(deserializedObject);
                }
                catch (Exception)
                {
                    internalServerErr = true;
                    callback(default(T));
                }

                if (internalServerErr)
                {
                    MessageBox.Show("There has been an error while retrieving data. Please try again later.");
                }
            };
        }
    }
}
