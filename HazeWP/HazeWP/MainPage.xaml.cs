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
                BindUI();
            }
            else 
            {
                this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            }
        }

        private void BindUI()
        {
            HistoryStack.Children.Clear();
            string locationId;
            Location.locationCollection.TryGetValue(LocationListPicker.SelectedItem as string, out locationId);
            RestClient restClient = new RestClient();
            restClient.Get<Haze>("http://myhaze-api.azurewebsites.net/api/hazemy/haze/" + locationId, (result) =>
            {
                PSINowText.Text = result.PSI;
                UpdateText.Text = result.TimeDiff;
                byte[] bytes = StringToByteArray(result.Color);
                PSINowEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, bytes[0], bytes[1], bytes[2]));

                restClient.Get<LinkedList<History>>("http://myhaze-api.azurewebsites.net/api/hazemy/haze/" + locationId + "/history", (innerResult) =>
                {
                    foreach (History history in innerResult)
                    {
                        HistoryControl historyControl = new HistoryControl();
                        historyControl.PSI = history.PSI;
                        historyControl.PSIDiff = history.PSIDiff;
                        historyControl.Color = history.Color;
                        historyControl.ColorDiff = history.ColorDiff;
                        historyControl.TimeDiff = history.TimeDiff;

                        HistoryStack.Children.Add(historyControl);
                        HistoryStack.Children.Add(historyControl);
                        LoadingAnimation.Stop();
                        UnloadingAnimation.Stop();
                        LoadingPanel.Visibility = Visibility.Collapsed;
                    }
                });
            });
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
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