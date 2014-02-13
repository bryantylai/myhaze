using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HazeAPI.Models;
using HtmlAgilityPack;

namespace HazeAPI.Services
{
    public class HazeService
    {
        private HttpClient httpClient;
        private const string BASE_URL = "http://apps.evozi.com/myapi/?loc=";

        public HazeService()
        {
            this.httpClient = new HttpClient();
        }

        internal async Task<City> CityDetailsById(string CityId, City city)
        {
            string result = await httpClient.GetStringAsync(BASE_URL + CityId);
            return await ProcessResultToCity(result, city);
        }

        private async Task<City> ProcessResultToCity(string result, City city)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(result);
            HtmlNode allNodes = htmlDocument.DocumentNode;

            IEnumerable<HtmlNode> divNodes = (from node in allNodes.DescendantsAndSelf()
                                           where node.Name.Equals("div")
                                           select node);

            String dateTime = "";
            foreach (HtmlNode divNode in divNodes)
            {
                if (divNode.HasAttributes && divNode.Attributes.Contains("class"))
                {
                    HtmlAttribute classAttribute = divNode.Attributes.AttributesWithName("class").First();
                    switch (classAttribute.Value)
                    {
                        case "psinow":
                            city.PSI = divNode.InnerText;
                            break;
                        case "psinowdate":
                           dateTime += divNode.InnerText.Replace("at ", "");
                            break;
                        case "psiolddate":
                            if (city.TimeDiff == null)
                            {
                                HtmlAttribute emptyStyleAttribute = divNode.Attributes.AttributesWithName("style").First();
                                if (emptyStyleAttribute.Value == "")
                                {
                                    dateTime += divNode.InnerText;
                                    TimeSpan diff = DateTime.UtcNow.Add(new TimeSpan(8,0,0)) - DateTime.SpecifyKind(DateTime.Parse(dateTime), DateTimeKind.Utc);
                                    if (diff.Hours < 1)
                                    {
                                        city.TimeDiff = diff.Minutes + " minute(s) ago";
                                    }
                                    else
                                    {
                                        city.TimeDiff = diff.Hours + " hour(s) ago";
                                    }
                                }
                            }
                            break;
                        case "psistatus":
                            HtmlAttribute styleAttribute = divNode.Attributes.AttributesWithName("style").First();
                            string style = styleAttribute.Value;
                            city.Color = style.Substring(style.IndexOf(":") + 1, 7);
                            break;
                        default:
                            break;
                    }
                }
            }

            IEnumerable<HtmlNode> aNodes = (from node in allNodes.DescendantsAndSelf()
                                              where node.Name.Equals("a")
                                              select node);

            foreach (HtmlNode aNode in aNodes)
            {
                if (aNode.HasAttributes && aNode.Attributes.Contains("class"))
                {
                    HtmlAttribute classAttribute = aNode.Attributes.AttributesWithName("class").First();
                    if (classAttribute.Value.Contains("button cam-selector"))
                    {
                        if (aNode.InnerText.Equals("LIVE weather info"))
                        {
                            string weatherURL = aNode.Attributes.AttributesWithName("href").First().Value;
                            city = await ProcessResultToWeatherCity(weatherURL, city);
                        }
                    }
                }
            }

            return city;
        }

        private async Task<City> ProcessResultToWeatherCity(string weatherURL, City city)
        {
            string result = await httpClient.GetStringAsync(weatherURL);
            return ProcessResultToWeather(result, city);
        }

        private City ProcessResultToWeather(string result, City city)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(result);
            HtmlNode allNodes = htmlDocument.DocumentNode;

            IEnumerable<HtmlNode> h2Nodes = (from node in allNodes.DescendantsAndSelf()
                                           where node.Name.Equals("h2")
                                           select node);

            foreach (HtmlNode h2Node in h2Nodes)
            {
                HtmlDocument h2Document = new HtmlDocument();
                h2Document.OptionFixNestedTags = true;
                h2Document.LoadHtml(h2Node.InnerHtml);
                HtmlNode h2HtmlNodes = h2Document.DocumentNode;

                HtmlNode imgNode = (from node in h2HtmlNodes.DescendantsAndSelf()
                                                 where node.Name.Equals("img")
                                                 select node).First();

                city.ImageURL = "http://apps.evozi.com/myapi/" + imgNode.Attributes.AttributesWithName("data-cfsrc").First().Value;
                string temp = h2Node.InnerText.Replace(" ", "");
                city.Temperature = temp.Insert(temp.IndexOf("°"), " ");
            }

            IEnumerable<HtmlNode> tdNodes = (from node in allNodes.DescendantsAndSelf()
                                             where node.Name.Equals("td")
                                             select node);

            foreach (HtmlNode tdNode in tdNodes)
            {
                switch (tdNode.InnerText)
                {
                    case "Pressure":
                        city.Pressure = tdNode.NextSibling.InnerText;
                        break;
                    case "Humidity":
                        city.Humidity = tdNode.NextSibling.InnerText;
                        break;
                }
            }

            return city;
        }
    }
}