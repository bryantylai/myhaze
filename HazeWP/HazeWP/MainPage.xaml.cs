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
                BindUI();
            }
            else 
            {
                this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            }
        }

        private void BindUI()
        {
            LoadingPanel.Visibility = Visibility.Visible;
            HistoryGridView.Visibility = Visibility.Collapsed;
            LoadingAnimation.Begin();
            HistoryStack.Children.Clear();
            string locationId;
            Location.locationCollection.TryGetValue(LocationListPicker.SelectedItem as string, out locationId);
            RestClient restClient = new RestClient();
            restClient.Get<HazeWithHistory>("http://myhaze-api.azurewebsites.net/api/hazemy/haze/history/" + locationId, (result) =>
            {
                if (result != null)
                {
                    LoadingAnimation.Stop();
                    UnloadingAnimation.Stop();
                    LoadingPanel.Visibility = Visibility.Collapsed;
                    HistoryGridView.Visibility = Visibility.Visible;

                    PSINowText.Text = result.Haze.PSI;
                    //UpdateText.Text = result.Haze.TimeDiff;
                    byte[] bytes = StringToByteArray(result.Haze.Color);
                    PSINowEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, bytes[0], bytes[1], bytes[2]));

                    LinkedList<HistoryControl> historyControlList = new LinkedList<HistoryControl>();
                    foreach (History history in result.Histories)
                    {
                        HistoryControl historyControl = new HistoryControl();
                        historyControl.PSI = history.PSI;
                        historyControl.PSIDiff = history.PSIDiff;
                        historyControl.Color = history.Color;
                        historyControl.ColorDiff = history.ColorDiff;
                        historyControl.TimeDiff = history.TimeDiff;
                        historyControlList.AddLast(historyControl);
                    }

                    foreach (HistoryControl historyControl in historyControlList)
                    {
                        HistoryStack.Children.Add(historyControl);
                    }
                }
            });
        }

        public static byte[] StringToByteArray(string hex)
        {
            byte[] bytes = new byte[3];
            int index = 0;
            for (int i = 1; i <= 5; i += 2)
            {
                bytes[index++] = Convert.ToByte(hex.Substring(i, 2), 16);
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
            task.Status = "Current API at " + (String)LocationListPicker.SelectedItem + " is " + PSINowText.Text + " via Haze MY #haze #myhaze http://aka.ms/hazemy";
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