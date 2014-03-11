using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HazeWP
{
    public partial class HistoryControl : UserControl
    {
        public string PSI { get; set; }
        public string PSIDiff { get; set; }
        public string Color { get; set; }
        public string ColorDiff { get; set; }
        public string TimeDiff { get; set; }

        public HistoryControl()
        {
            InitializeComponent();
        }
    }
}
