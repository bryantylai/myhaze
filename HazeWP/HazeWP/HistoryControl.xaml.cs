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
        public static readonly DependencyProperty PSIProperty = DependencyProperty.Register(
                                             "PSI",
                                             typeof(string),
                                             typeof(HistoryControl),
                                             new PropertyMetadata(""));
        public static readonly DependencyProperty PSIDiffProperty = DependencyProperty.Register(
                                             "PSIDiff",
                                             typeof(string),
                                             typeof(HistoryControl),
                                             new PropertyMetadata(""));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
                                             "Color",
                                             typeof(string),
                                             typeof(HistoryControl),
                                             new PropertyMetadata(""));
        public static readonly DependencyProperty ColorDiffProperty = DependencyProperty.Register(
                                             "ColorDiff",
                                             typeof(string),
                                             typeof(HistoryControl),
                                             new PropertyMetadata(""));
        public static readonly DependencyProperty TimeDiffProperty = DependencyProperty.Register(
                                             "TimeDiff",
                                             typeof(string),
                                             typeof(HistoryControl),
                                             new PropertyMetadata(""));
        public string PSI
        {
            get
            {
                return GetValue(PSIProperty) as string;
            }
            set
            {
                SetValue(PSIProperty, value);
            }
        }
        public string PSIDiff
        {
            get
            {
                return GetValue(PSIDiffProperty) as string;
            }
            set
            {
                SetValue(PSIDiffProperty, value);
            }
        }
        public string Color
        {
            get
            {
                return GetValue(ColorProperty) as string;
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        public string ColorDiff
        {
            get
            {
                return GetValue(ColorDiffProperty) as string;
            }
            set
            {
                SetValue(ColorDiffProperty, value);
            }
        }
        public string TimeDiff
        {
            get
            {
                return GetValue(TimeDiffProperty) as string;
            }
            set
            {
                SetValue(TimeDiffProperty, value);
            }
        }

        public HistoryControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}
