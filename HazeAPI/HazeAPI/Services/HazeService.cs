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
                            city.Time = divNode.InnerText;
                            break;
                        case "psiolddate":
                            if (city.Date == null)
                            {
                                HtmlAttribute emptyStyleAttribute = divNode.Attributes.AttributesWithName("style").First();
                                if (emptyStyleAttribute.Value == "")
                                {
                                    city.Date = divNode.InnerText;
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

        internal async Task<IEnumerable<State>> ListCityInDetails()
        {
            HashSet<State> StateCollection = new HashSet<State>();

            State Johor = new State() { Name = "Johor", Cities = new HashSet<City>() };
            Johor.Cities.Add(new City() { ID = "kota_tinggi", Location = "Kota Tinggi"});
            Johor.Cities.Add(new City() { ID = "larkin_lama", Location = "Larkin Lama"});
            Johor.Cities.Add(new City() { ID = "muar", Location = "Muar" });
            Johor.Cities.Add(new City() { ID = "pasir_gudang", Location = "Pasir Gudang"});

            State Kedah = new State() { Name = "Kedah", Cities = new HashSet<City>() };
            Kedah.Cities.Add(new City() { ID = "alor_setar", Location = "Alor Setar"});
            Kedah.Cities.Add(new City() { ID = "bakar_arang", Location = "Bakar Arang, Sg. Petani"});
            Kedah.Cities.Add(new City() { ID = "langkawi", Location = "Langkawi"});

            State Kelantan = new State() { Name = "Kelantan", Cities = new HashSet<City>() };
            Kelantan.Cities.Add(new City() { ID = "kota_bharu", Location = "Kota Bharu"});
            Kelantan.Cities.Add(new City() { ID = "tanah_merah", Location = "Tanah Merah"});

            State Melaka = new State() { Name = "Melaka", Cities = new HashSet<City>() };
            Melaka.Cities.Add(new City() { ID = "bandaraya_melaka", Location = "Bandaraya Melaka"});
            Melaka.Cities.Add(new City() { ID = "bukit_rambai", Location = "Bukit Rambai"});

            State NSembilan = new State() { Name = "Negeri Sembilan", Cities = new HashSet<City>() };
            NSembilan.Cities.Add(new City() { ID = "nilai", Location = "Nilai"});
            NSembilan.Cities.Add(new City() { ID = "port_dickson", Location = "Port Dickson"});
            NSembilan.Cities.Add(new City() { ID = "seremban", Location = "Seremban"});

            State Pahang = new State() { Name = "Pahang", Cities = new HashSet<City>() };
            Pahang.Cities.Add(new City() { ID = "balok_baru", Location = "Balok Baru, Kuantan"});
            Pahang.Cities.Add(new City() { ID = "indera_mahkota", Location = "Indera Mahkota, Kuantan"});
            Pahang.Cities.Add(new City() { ID = "jerantut", Location = "Jerantut"});

            State Perak = new State() { Name = "Perak", Cities = new HashSet<City>() };
            Perak.Cities.Add(new City() { ID = "jalan_tasek", Location = "Jalan Tasek, Ipoh"});
            Perak.Cities.Add(new City() { ID = "air_putih", Location = "Kg. Air Putih, Taiping"});
            Perak.Cities.Add(new City() { ID = "sk_jalan_pegoh", Location = "S K Jalan Pegoh, Ipoh"});
            Perak.Cities.Add(new City() { ID = "seri_manjung", Location = "Seri Manjung"});
            Perak.Cities.Add(new City() { ID = "tanjung_malim", Location = "Tanjung Malim"});

            State Perlis = new State() { Name = "Perlis", Cities = new HashSet<City>() };
            Perlis.Cities.Add(new City() { ID = "kangar", Location = "Kangar"});

            State Pinang = new State() { Name = "Pulau Pinang", Cities = new HashSet<City>() };
            Pinang.Cities.Add(new City() { ID = "perai", Location = "Perai"});
            Pinang.Cities.Add(new City() { ID = "seberang_jaya_2", Location = "Seberang Jaya 2"});
            Pinang.Cities.Add(new City() { ID = "usm", Location = "USM"});

            State Sabah = new State() { Name = "Sabah", Cities = new HashSet<City>() };
            Sabah.Cities.Add(new City() { ID = "keningau", Location = "Keningau"});
            Sabah.Cities.Add(new City() { ID = "kota_kinabalu", Location = "Kota Kinabalu"});
            Sabah.Cities.Add(new City() { ID = "sandakan", Location = "Sandakan"});
            Sabah.Cities.Add(new City() { ID = "tawau", Location = "Tawau"});

            State Sarawak = new State() { Name = "Sarawak", Cities = new HashSet<City>() };
            Sarawak.Cities.Add(new City() { ID = "bintulu", Location = "Bintulu"});
            Sarawak.Cities.Add(new City() { ID = "ilp_miri", Location = "ILP Miri"});
            Sarawak.Cities.Add(new City() { ID = "kapit", Location = "Kapit"});
            Sarawak.Cities.Add(new City() { ID = "kuching", Location = "Kuching"});
            Sarawak.Cities.Add(new City() { ID = "limbang", Location = "Limbang"});
            Sarawak.Cities.Add(new City() { ID = "miri", Location = "Miri"});
            Sarawak.Cities.Add(new City() { ID = "samarahan", Location = "Samarahan"});
            Sarawak.Cities.Add(new City() { ID = "sarikei", Location = "Sarikei"});
            Sarawak.Cities.Add(new City() { ID = "sibu", Location = "Sibu"});
            Sarawak.Cities.Add(new City() { ID = "sri_aman", Location = "Sri Aman"});

            State Selangor = new State() { Name = "Selangor", Cities = new HashSet<City>() };
            Selangor.Cities.Add(new City() { ID = "banting", Location = "Banting"});
            Selangor.Cities.Add(new City() { ID = "kuala_selangor", Location = "Kuala Selangor"});
            Selangor.Cities.Add(new City() { ID = "pelabuhan_klang", Location = "Pelabuhan Klang"});
            Selangor.Cities.Add(new City() { ID = "petaling_jaya", Location = "(Puchong) Petaling Jaya"});
            Selangor.Cities.Add(new City() { ID = "shah_alam", Location = "Shah Alam"});

            State Terengganu = new State() { Name = "Terengganu", Cities = new HashSet<City>() };
            Terengganu.Cities.Add(new City() { ID = "kemaman", Location = "Kemaman"});
            Terengganu.Cities.Add(new City() { ID = "kuala_terengganu", Location = "Kuala Terengganu"});

            State Wilayah = new State() { Name = "Wilayah Persekutuan", Cities = new HashSet<City>() };
            Wilayah.Cities.Add(new City() { ID = "batu_muda", Location = "Batu Muda, Kuala Lumpur"});
            Wilayah.Cities.Add(new City() { ID = "cheras", Location = "Cheras, Kuala Lumpur"});
            Wilayah.Cities.Add(new City() { ID = "labuan", Location = "Labuan"});
            Wilayah.Cities.Add(new City() { ID = "putrajaya", Location = "Putrajaya"});

            StateCollection.Add(Johor);
            StateCollection.Add(Kedah);
            StateCollection.Add(Kelantan);
            StateCollection.Add(Melaka);
            StateCollection.Add(NSembilan);
            StateCollection.Add(Pahang);
            StateCollection.Add(Perak);
            StateCollection.Add(Perlis);
            StateCollection.Add(Pinang);
            StateCollection.Add(Sabah);
            StateCollection.Add(Sarawak);
            StateCollection.Add(Selangor);
            StateCollection.Add(Terengganu);
            StateCollection.Add(Wilayah);

            HashSet<State> states = new HashSet<State>();

            foreach (State state in StateCollection)
            {
                State newState = new State();
                newState.Name = state.Name;
                newState.Cities = new HashSet<City>();
                foreach (City city in state.Cities)
                {
                    City newCity = await this.CityDetailsById(city.ID, city);
                    newState.Cities.Add(newCity);
                }
                states.Add(newState);
            }

            return states;
        }
    }
}