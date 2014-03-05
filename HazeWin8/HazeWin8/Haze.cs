using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazeWin8
{
    public class State
    {
        public string Name { get; set; }
        public ObservableCollection<Haze> HazeCollection { get; set; }
    }

    public class Haze
    {
        public Haze()
        {
            PSI = "??";
            TimeDiff = "?? ago";
            Color = "#666666";
        }

        public string ID { get; set; }
        public string Location { get; set; }
        public string PSI { get; set; }
        public string TimeDiff { get; set; }
        public string Color { get; set; }
    }
}
