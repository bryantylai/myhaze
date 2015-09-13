using HazeAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HazeAPI.Services
{
    public class UpdateService
    {
        public async Task<bool> Update()
        {
            var entities = new Models.HazeMYEntities();

            if (entities.Locations.Count() == 0)
            {
                PreLoad(entities);
            }

            DateTime currentMalaysiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            int index = GetCurrentIndexByMalaysiaTime(currentMalaysiaTime);
            HtmlNode documentNode = null;

            int retryCount = 0;
            Exception exception = null;
            while (documentNode == null)
            {
                try
                {
                    documentNode = await GetHtml(string.Format(@"http://apims.doe.gov.my/apims/hourly{0}.php", index));
                }
                catch (Exception ex)
                {
                    exception = ex;
                    retryCount++;
                }

                if (retryCount > 5)
                    throw exception;
            }

            if (documentNode != null)
            {
                HtmlNode hazeTable = documentNode.Descendants("table").FirstOrDefault((n) => n.HasAttributes && n.Attributes.AttributesWithName("class").First().Value.Equals("table1"));
                List<Location> locations = entities.Locations.ToList();
                foreach (Location location in locations)
                {
                    string locationToGet = location.Name;
                    List<HtmlNode> hazeNodes = GetHazeNodes(locationToGet, hazeTable);
                    List<string> hazeValues = hazeNodes.Select((n) => n.InnerText).ToList();

                    Dictionary<DateTime, string> hazeTimeTable = PopulateHazeTimeTable(index, hazeValues, currentMalaysiaTime.Date);

                    foreach (KeyValuePair<DateTime, string> itemPair in hazeTimeTable)
                    {
                        Haze existingValue = entities.Hazes.Where(h => h.Location.Name == location.Name && h.RecordDate == currentMalaysiaTime.Date && h.RecordHour == itemPair.Key.Hour).FirstOrDefault();
                        if (existingValue == null)
                        {
                            location.LastUpdatedAt = itemPair.Key;

                            Haze haze = new Haze();
                            haze.Location = location;
                            haze.Id = Guid.NewGuid();
                            var latestHazeValueAsCharArray = itemPair.Value.ToCharArray();

                            string code = latestHazeValueAsCharArray.LastOrDefault().ToString();
                            if (Regex.IsMatch(code, "[*abcd&]"))
                            {
                                haze.Code = code;
                            }

                            string hazeValueAsString = Regex.Replace(itemPair.Value, "[*abcd&]", string.Empty);
                            int hazeValueAsInt = int.Parse(hazeValueAsString);
                            haze.PSI = hazeValueAsInt;

                            haze.RecordDate = itemPair.Key.Date;
                            haze.RecordHour = itemPair.Key.Hour;

                            entities.Hazes.Add(haze);
                        }
                    }
                }

                if(entities.SaveChanges() > 0)
                {
                    DateTime deletionDate =  currentMalaysiaTime.Date.AddDays(-3);
                    List<Haze> hazesToDeleteList = entities.Hazes.Where(h => h.RecordDate < deletionDate).ToList();
                    foreach (Haze hazeToDelete in hazesToDeleteList)
                    {
                        entities.Hazes.Remove(hazeToDelete);
                    }

                    entities.SaveChanges();
                }

                return true;
            }

            return false;
        }

        private Dictionary<DateTime, string> PopulateHazeTimeTable(int index, List<string> hazeValues, DateTime malaysiaDate)
        {
            DateTime initialTime = malaysiaDate;
            Dictionary<DateTime, string> hazeTimeTable = new Dictionary<DateTime, string>();
            switch (index)
            {
                case 2:
                    initialTime = malaysiaDate.AddHours(6);
                    break;
                case 3:
                    initialTime = malaysiaDate.AddHours(12);
                    break;
                case 4:
                    initialTime = malaysiaDate.AddHours(18);
                    break;
            }

            foreach (string hazeValue in hazeValues)
            {
                if (!hazeValue.Equals("#"))
                    hazeTimeTable.Add(initialTime, hazeValue);
                initialTime = initialTime.AddHours(1);
            }

            return hazeTimeTable;
        }

        private void PreLoad(HazeMYEntities entities)
        {
            foreach (var item in LocationList)
            {
                Location location = new Location();
                location.Id = Guid.NewGuid();
                location.Code = item.Key;
                location.Name = item.Value;

                entities.Locations.Add(location);
            }

            entities.SaveChanges();
        }

        #region Location

        private readonly Dictionary<string, string> LocationList = new Dictionary<string, string>()
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
            {"pelabuhan_klang", "Pelabuhan Kelang"},
            {"petaling_jaya", "Petaling Jaya"},
            {"shah_alam", "Shah Alam"},

            {"kemaman", "Kemaman"},
            {"kuala_terengganu", "Kuala Terengganu"},
            
            {"labuan", "Labuan"},
            {"putrajaya", "Putrajaya"},
        };

        #endregion

        private List<HtmlNode> GetHazeNodes(string locationToGet, HtmlNode hazeTable)
        {
            HtmlNode[] nodes = new HtmlNode[6];
            var cityNodes = hazeTable.Descendants("td").ToList();
            var cityNode = cityNodes.FirstOrDefault((n) => n.InnerText.Contains(locationToGet));
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

        private async Task<HtmlNode> GetHtml(string url)
        {
            var currentUri = new Uri(url, UriKind.Absolute);
            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(currentUri);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(html);
            return htmlDocument.DocumentNode;
        }

        private int GetCurrentIndexByMalaysiaTime(DateTime currentMalaysiaTime)
        {
            if (currentMalaysiaTime.Hour == 0)
            {
                currentMalaysiaTime = currentMalaysiaTime.AddHours(-1);
                return 4;
            }
            else if (currentMalaysiaTime.Hour <= 6)
            {
                return 1;
            }
            else if (currentMalaysiaTime.Hour <= 12)
            {
                return 2;
            }
            else if (currentMalaysiaTime.Hour <= 18)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }
}
