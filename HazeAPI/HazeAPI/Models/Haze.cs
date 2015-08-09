using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HazeAPI.Models
{
    public class City
    {
        public string ID { get; set; }
        public string Location { get; set; }
        public string PSI { get; set; }
        public string TimeDiff { get; set; }
        public string Color { get; set; }
        public string ImageURL { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
    }

    public class HazeWithHistory
    {
        public Haze Haze { get; set; }
        public LinkedList<History> Histories { get; set; }
    }

    public class Haze
    {
        public string ID { get; set; }
        public string Location { get; set; }
        public string PSI { get; set; }
        public string TimeDiff { get; set; }
        public string Color { get; set; }
    }

    public class History
    {
        public string PSI { get; set; }
        public string PSIDiff { get; set; }
        public string Color { get; set; }
        public string ColorDiff { get; set; }
        public string TimeDiff { get; set; }
    }

    public class HazeWithHistoryContainer
    {
        public ExceptionLite Exception { get; set; }
        public HazeWithHistory HazeWithHistory { get; set; }
    }

    public class ExceptionLite
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}