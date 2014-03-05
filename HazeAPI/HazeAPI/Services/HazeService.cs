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
            return ProcessResultToCity(result, city);
        }

        private City ProcessResultToCity(string result, City city)
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

            return city;
        }

        internal async Task<Haze> HazeDetailsById(string hazeId, Haze haze)
        {
            string result = await httpClient.GetStringAsync(BASE_URL + hazeId);
            return ProcessResultToHaze(result, haze);
        }

        private Haze ProcessResultToHaze(string result, Haze haze)
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
                            haze.PSI = divNode.InnerText;
                            break;
                        case "psinowdate":
                            dateTime += divNode.InnerText.Replace("at ", "");
                            break;
                        case "psiolddate":
                            if (haze.TimeDiff == null)
                            {
                                HtmlAttribute emptyStyleAttribute = divNode.Attributes.AttributesWithName("style").First();
                                if (emptyStyleAttribute.Value == "")
                                {
                                    dateTime += divNode.InnerText;
                                    TimeSpan diff = DateTime.UtcNow.Add(new TimeSpan(8, 0, 0)) - DateTime.SpecifyKind(DateTime.Parse(dateTime), DateTimeKind.Utc);
                                    if (diff.Hours < 1)
                                    {
                                        haze.TimeDiff = diff.Minutes + " minute(s) ago";
                                    }
                                    else
                                    {
                                        haze.TimeDiff = diff.Hours + " hour(s) ago";
                                    }
                                }
                            }
                            break;
                        case "psistatus":
                            HtmlAttribute styleAttribute = divNode.Attributes.AttributesWithName("style").First();
                            string style = styleAttribute.Value;
                            haze.Color = style.Substring(style.IndexOf(":") + 1, 7);
                            break;
                        default:
                            break;
                    }
                }
            }

            return haze;
        }
    }
}