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

        [Obsolete]
        internal async Task<City> CityDetailsById(string CityId, City city)
        {
            string result = await httpClient.GetStringAsync(BASE_URL + CityId);
            return ProcessResultToCity(result, city);
        }

        internal async Task<Haze> HazeDetailsById(string hazeId, Haze haze)
        {
            string result = await httpClient.GetStringAsync(BASE_URL + hazeId);
            return ProcessResultToHaze(result, haze);
        }

        internal async Task<HazeWithHistory> HazeHistoryById(string hazeId, HazeWithHistory hazeWithHistory)
        {
            string result = await httpClient.GetStringAsync(BASE_URL + hazeId);
            ProcessResultToHaze(result, hazeWithHistory.Haze);
            ProcessResultToHazeHistory(result, hazeWithHistory.Histories);
            return hazeWithHistory;
        }

        [Obsolete]
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
                                    DateTime psiDateTime = DateTime.Parse(dateTime);
                                    int utcHour = DateTime.UtcNow.Hour + 8;
                                    if (utcHour < 24)
                                    {
                                        psiDateTime = new DateTime(psiDateTime.Year, psiDateTime.Month, psiDateTime.Day - 1, psiDateTime.Hour, psiDateTime.Minute, psiDateTime.Second);
                                    }
                                    TimeSpan diff = DateTime.UtcNow.Add(new TimeSpan(8, 0, 0)) - DateTime.SpecifyKind(psiDateTime, DateTimeKind.Utc);
                                    if (diff.Days > 0)
                                    {
                                        haze.TimeDiff = diff.Days + " day(s) ago";
                                    }
                                    else if (diff.Hours < 1)
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

        private LinkedList<History> ProcessResultToHazeHistory(string result, LinkedList<History> historyList)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(result);
            HtmlNode allNodes = htmlDocument.DocumentNode;

            IEnumerable<HtmlNode> divNodes = (from node in allNodes.DescendantsAndSelf()
                                              where node.Name.Equals("div")
                                              select node);

            foreach (HtmlNode divNode in divNodes)
            {
                if (divNode.HasAttributes && divNode.Attributes.Contains("class"))
                {
                    HtmlAttribute classAttribute = divNode.Attributes.AttributesWithName("class").First();
                    if (divNode.Attributes.Count == 1 && classAttribute.Value.Equals("row"))
                    {
                        HtmlDocument divNodeHtml = new HtmlDocument();
                        divNodeHtml.OptionFixNestedTags = true;
                        divNodeHtml.LoadHtml(divNode.InnerHtml);
                        HtmlNode parentRowDivNode = divNodeHtml.DocumentNode;

                        IEnumerable<HtmlNode> rowDivNodes = (from node in parentRowDivNode.DescendantsAndSelf()
                                                          where node.Name.Equals("div")
                                                          select node);

                        string dateTime = "";
                        History history = new History();
                        foreach (HtmlNode rowDivNode in rowDivNodes)
                        {
                            HtmlAttribute rowDivClassAttribute = rowDivNode.Attributes.AttributesWithName("class").First();
                            switch (rowDivClassAttribute.Value)
                            {
                                case "psiold":
                                    history.PSI = rowDivNode.InnerText;
                                    HtmlAttribute styleAttribute = rowDivNode.Attributes.AttributesWithName("style").First();
                                    string style = styleAttribute.Value;
                                    history.Color = style.Substring(style.IndexOf(":") + 1, 7);
                                    break;
                                case "psiolddate":
                                    if (String.IsNullOrEmpty(dateTime))
                                    {
                                        dateTime = rowDivNode.InnerText.Replace("at ", "");
                                    }
                                    else
                                    {
                                        HtmlAttribute emptyStyleAttribute = rowDivNode.Attributes.AttributesWithName("style").First();
                                        if (emptyStyleAttribute.Value == "")
                                        {
                                            dateTime += rowDivNode.InnerText; DateTime psiDateTime = DateTime.Parse(dateTime);
                                            int utcHour = DateTime.UtcNow.Hour + 8;
                                            if (utcHour < 24)
                                            {
                                                psiDateTime = new DateTime(psiDateTime.Year, psiDateTime.Month, psiDateTime.Day - 1, psiDateTime.Hour, psiDateTime.Minute, psiDateTime.Second);
                                            }
                                            TimeSpan diff = DateTime.UtcNow.Add(new TimeSpan(8, 0, 0)) - DateTime.SpecifyKind(psiDateTime, DateTimeKind.Utc);
                                            if (diff.Days > 0)
                                            {
                                                history.TimeDiff = diff.Days + " day(s) ago";
                                            } 
                                            else if (diff.Hours < 1)
                                            {
                                                history.TimeDiff = diff.Minutes + " minute(s) ago";
                                            }
                                            else
                                            {
                                                history.TimeDiff = diff.Hours + " hour(s) ago";
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        historyList.AddLast(history);
                    }
                }
            }

            IEnumerator<History> historyEnumerator = historyList.GetEnumerator();
            IEnumerator<History> historyToCompareEnumerator = historyList.GetEnumerator();
            historyEnumerator.MoveNext();
            historyToCompareEnumerator.MoveNext();
            while (historyToCompareEnumerator.MoveNext())
            {
                History currentHistory = historyEnumerator.Current;
                History currentHistoryToCompare = historyToCompareEnumerator.Current;
                int psiDiff = Convert.ToInt32(currentHistoryToCompare.PSI) - Convert.ToInt32(currentHistory.PSI);
                
                if (psiDiff > 0)
                {
                    currentHistory.ColorDiff = "#66FF99";
                    currentHistory.PSIDiff = psiDiff.ToString();
                }
                else if (psiDiff < 0)
                {
                    currentHistory.ColorDiff = "#CC0033";
                    currentHistory.PSIDiff = Math.Abs(psiDiff).ToString();
                }
                
                historyEnumerator.MoveNext();
            }

            return historyList;
        }
    }
}