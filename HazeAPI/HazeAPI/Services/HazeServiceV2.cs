using HazeAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace HazeAPI.Services
{
    public class HazeServiceV2
    {
        #region Location

        private readonly Dictionary<string, string> Location = new Dictionary<string, string>()
        {
            {"kota_tinggi", "Kota Tinggi"},
            {"larkin_lama", "Larkin Lama"},
            {"muar", "Muar"},
            {"pasir_gudang", "Pasir Gudang"},

            {"alor_setar", "Alor Setar"},
            {"bakar_arang", "Bakar Arang, Sg. Petani"},
            {"langkawi", "Langkawi"},

            {"kota_bharu", "Kota Bharu"},
            {"tanah_merah", "Tanah Merah"},

            {"batu_muda", "Batu Muda,Kuala Lumpur"},
            {"cheras", "Cheras,Kuala Lumpur"},

            {"bandaraya_melaka", "Bandaraya Melaka"},
            {"bukit_rambai", "Bukit Rambai"},
            
            {"nilai", "Nilai"},
            {"port_dickson", "Port Dickson"},
            {"seremban", "Seremban"},

            {"balok_baru", "Balok Baru, Kuantan"},
            {"indera_mahkota", "Indera Mahkota, Kuantan"},
            {"jerantut", "Jerantut"},
            
            {"jalan_tasek", "Jalan Tasek, Ipoh"},
            {"air_putih", "Kg. Air Putih, Taiping"},
            {"sk_jalan_pegoh", "S K Jalan Pegoh, Ipoh"},
            {"seri_manjung", "Seri Manjung"},
            {"tanjung_malim", "Tanjung Malim"},

            {"kangar", "Kangar"},
            
            {"perai", "Perai"},
            {"seberang_jaya_2", "Seberang Jaya 2, Perai"},
            {"usm", "USM"},
            
            {"keningau", "Keningau"},
            {"kota_kinabalu", "Kota Kinabalu"},
            {"sandakan", "Sandakan"},
            {"tawau", "Tawau"},
            
            {"bintulu", "Bintulu"},
            {"ilp_miri", "ILP Miri"},
            {"kapit", "Kapit"},
            {"kuching", "Kuching"},
            {"limbang", "Limbang"},
            {"miri", "Miri"},
            {"samarahan", "Samarahan"},
            {"sarikei", "Sarikei"},
            {"sibu", "Sibu"},
            {"sri_aman", "Sri Aman"},

            {"banting", "Banting"},
            {"kuala_selangor", "Kuala Selangor"},
            {"pelabuhan_klang", "Pelabuhan Klang"},
            {"petaling_jaya", "Petaling Jaya"},
            {"shah_alam", "Shah Alam"},

            {"kemaman", "Kemaman"},
            {"kuala_terengganu", "Kuala Terengganu"},
            
            {"labuan", "Labuan"},
            {"putrajaya", "Putrajaya"},
        };

        #endregion

        private HttpClient client;
        public HazeServiceV2()
        {
            this.client = new HttpClient();
        }

        internal async Task<Haze> HazeDetailsById(string hazeId, Haze haze, DateTime? msiaNow = null)
        {
            string url = BuildHazeApiUrl(ref msiaNow);
            HtmlNode documentNode = await GetHtmlDocumentNode(msiaNow.Value, url);

            if (documentNode != null)
            {
                var hazeTable = documentNode.Descendants("table").FirstOrDefault((n) => n.HasAttributes && n.Attributes.AttributesWithName("class").First().Value.Equals("table1"));
                string locationToGet;
                if (Location.TryGetValue(hazeId, out locationToGet))
                {
                    var hazeNodes = GetHazeNodes(locationToGet, hazeTable);
                    var hazeValues = hazeNodes.Select((n) => n.InnerText);
                    if (hazeValues.All((n) => n.Equals("#")))
                    {
                        // Get from previous hours
                        haze = await HazeDetailsById(hazeId, haze, msiaNow.Value.AddHours(-8));
                    }
                    else
                    {
                        haze.ID = hazeId;
                        haze.Location = locationToGet;
                        var latestHazeValue = hazeValues.LastOrDefault((n) => !n.Equals("#"));
                        string hazeValueAsString = Regex.Replace(latestHazeValue, "[*abcd&]", string.Empty);
                        int hazeValueAsInt = int.Parse(hazeValueAsString);
                        haze.Color = GetHazeColor(hazeValueAsInt);
                        haze.PSI = hazeValueAsString;
                        haze.TimeDiff = GetHazeTime(msiaNow.Value, hazeValues.ToList().LastIndexOf(latestHazeValue));
                    }

                    return haze;
            
                }
            }

            throw new HttpRequestException("Unable to connect to remote server.");
        }

        internal async Task<HazeWithHistory> HazeHistoryById(string hazeId, HazeWithHistory hazeWithHistory, DateTime? msiaNow = null)
        {
            string url = BuildHazeApiUrl(ref msiaNow);
            HtmlNode documentNode = await GetHtmlDocumentNode(msiaNow.Value, url);

            if (documentNode != null)
            {
                var hazeTable = documentNode.Descendants("table").FirstOrDefault((n) => n.HasAttributes && n.Attributes.AttributesWithName("class").First().Value.Equals("table1"));
                string locationToGet;
                if (Location.TryGetValue(hazeId, out locationToGet))
                {
                    var hazeNodes = GetHazeNodes(locationToGet, hazeTable);
                    var hazeValues = hazeNodes.Select((n) => n.InnerText);
                    if (hazeValues.All((n) => n.Equals("#")))
                    {
                        // Get from previous hours
                        hazeWithHistory = await HazeHistoryById(hazeId, hazeWithHistory, msiaNow.Value.AddHours(-8));
                    }
                    else
                    {
                        for (int index = 5; index >= 0; index--)
                        {
                            string hazeValue = hazeValues.ElementAt(index);
                            if (!hazeValue.Equals("#"))
                            {
                                History history = new History();

                                string hazeValueAsString = Regex.Replace(hazeValue, "[*abcd&]", string.Empty);
                                int hazeValueAsInt = int.Parse(hazeValueAsString);
                                history.Color = GetHazeColor(hazeValueAsInt);
                                history.PSI = hazeValueAsString;
                                history.TimeDiff = GetHazeTime(msiaNow.Value, index);

                                hazeWithHistory.Histories.AddLast(history);
                            }
                        }
                    }

                    if (hazeWithHistory.Histories.Count < 4)
                    {
                        HazeWithHistory hazeWithHistory_More = new HazeWithHistory();
                        hazeWithHistory_More.Haze = new Haze();
                        hazeWithHistory_More.Histories = new LinkedList<History>();
                        hazeWithHistory_More = await HazeHistoryById(hazeId, hazeWithHistory_More, msiaNow.Value.AddHours(-8));

                        foreach (History history in hazeWithHistory_More.Histories)
                        {
                            hazeWithHistory.Histories.AddLast(history);
                        }
                    }

                    IEnumerator<History> historyEnumerator = hazeWithHistory.Histories.GetEnumerator();
                    IEnumerator<History> historyToCompareEnumerator = hazeWithHistory.Histories.GetEnumerator();
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

                    hazeWithHistory.Haze.ID = hazeId;
                    hazeWithHistory.Haze.Location = locationToGet;
                    History latest = hazeWithHistory.Histories.First();
                    hazeWithHistory.Haze.Color = latest.Color;
                    hazeWithHistory.Haze.PSI = latest.PSI;
                    hazeWithHistory.Haze.TimeDiff = latest.TimeDiff;

                    return hazeWithHistory;
                }
            }

            throw new HttpRequestException("Unable to connect to remote server.");
        }

        private string GetHazeTime(DateTime msiaTime, int index)
        {
            string[] times;
            if (msiaTime.Hour <= 5)
            {
                times = new string[]
                {
                    "12 AM",
                    "1 AM",
                    "2 AM",
                    "3 AM",
                    "4 AM",
                    "5 AM"
                };
            }
            else if (msiaTime.Hour <= 11)
            {
                times = new string[]
                {
                    "6 AM",
                    "7 AM",
                    "8 AM",
                    "9 AM",
                    "10 AM",
                    "11 AM"
                };
            }
            else if (msiaTime.Hour <= 17)
            {
                times = new string[]
                {
                    "12 PM",
                    "1 PM",
                    "2 PM",
                    "3 PM",
                    "4 PM",
                    "5 PM"
                };
            }
            else
            {
                times = new string[]
                {
                    "6 PM",
                    "7 PM",
                    "8 PM",
                    "9 PM",
                    "10 PM",
                    "11 PM"
                };
            }

            return times[index];
        }

        private IEnumerable<HtmlNode> GetHazeNodes(string locationToGet, HtmlNode hazeTable)
        {
            HtmlNode[] nodes = new HtmlNode[6];
            var cityNode = hazeTable.Descendants("td").FirstOrDefault((n) => n.InnerText.Equals(locationToGet));
            if (cityNode != null)
            {
                nodes[0] = cityNode.NextSibling;

                for (int index = 1; index < 6; index++)
                {
                    nodes[index] = nodes[index - 1].NextSibling;
                }
            }

            return nodes.ToList();
        }

        private async Task<HtmlNode> GetHtmlDocumentNode(DateTime msiaNow, string url)
        {
            HtmlNode documentNode = null;

            HttpResponseMessage response = await client.GetAsync(url + msiaNow.Date.ToString("yyyy-MM-dd"));
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.OptionFixNestedTags = true;
                    htmlDocument.LoadHtml(result);
                    documentNode = htmlDocument.DocumentNode;
                }
            }

            return documentNode;
        }

        private string BuildHazeApiUrl(ref DateTime? msiaNow)
        {
            if (!msiaNow.HasValue)
                msiaNow = DateTime.UtcNow.AddHours(8);
            string url = "";

            if (msiaNow.Value.Hour <= 5)
            {
                url = "http://apims.doe.gov.my/apims/hourly1.php?";
            }
            else if (msiaNow.Value.Hour <= 11)
            {
                url = "http://apims.doe.gov.my/apims/hourly2.php?";
            }
            else if (msiaNow.Value.Hour <= 17)
            {
                url = "http://apims.doe.gov.my/apims/hourly3.php?";
            }
            else
            {
                url = "http://apims.doe.gov.my/apims/hourly4.php?";
            }
            return url;
        }

        private string GetHazeColor(int value)
        {
            if (value < 50)
            {
                return "#78BA00";
            }
            else if (value < 100)
            {
                return "#F4B300";
            }
            else
            {
                return "#E02611";
            }
        }
    }
}