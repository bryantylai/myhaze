﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HazeAPI.Models
{
    public class State
    {
        public string Name { get; set; }
        public HashSet<City> Cities { get; set; }
    }

    public class City
    {
        public string ID { get; set; }
        public string Location { get; set; }
        public string PSI { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Color { get; set; }
        public string ImageURL { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
    }
}