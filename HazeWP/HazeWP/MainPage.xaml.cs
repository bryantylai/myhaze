using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using HazeWP.Resources;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace HazeWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings isolatedStorageSettings;
        private bool isSelectionMade = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;
            LocationListPicker.ItemsSource = Location.locationCollection.Keys;
            LocationListPicker.SelectionChanged += LocationListPicker_SelectionChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string defaultLocation;
            bool isNotFirstLaunch = this.isolatedStorageSettings.TryGetValue<string>("Location", out defaultLocation);
            if (isNotFirstLaunch)
            {
                LocationListPicker.SelectedItem = defaultLocation;
                LoadingAnimation.Begin();
            }
            else 
            {
                this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            }
        }

        private void BindUI()
        {

        }

        #region Loading Animation

        private void LoadingAnimation_Completed(object sender, EventArgs e)
        {
            UnloadingAnimation.Begin();
        }

        private void UnloadingAnimation_Completed(object sender, EventArgs e)
        {
            LoadingAnimation.Begin();
        }

        #endregion

        private void SettingsButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void ShareButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShareStatusTask task = new ShareStatusTask();
            task.Status = "Current API at " + (String)LocationListPicker.SelectedItem + " is " + PSINowText.Text + ". Updated at " + UpdateText.Text + " via MY Haze #haze #myhaze http://aka.ms/hazemy";
            task.Show();
        }

        private void RefreshButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BindUI();
        }

        private void HistoryTextGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (HistoryGridView.Visibility == Visibility.Collapsed)
            {
                HistoryGridView.Visibility = Visibility.Visible;
            }
            else
            {
                HistoryGridView.Visibility = Visibility.Collapsed;
            }
        }

        private void LocationListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isSelectionMade)
            {
                if (LocationListPicker.SelectedItem != null)
                {
                    string location = LocationListPicker.SelectedItem as string;
                    this.isolatedStorageSettings.Remove("Location");
                    this.isolatedStorageSettings.Add("Location", location);
                    this.isolatedStorageSettings.Save();
                }
            }

            isSelectionMade = true;
        }

        
    }
}