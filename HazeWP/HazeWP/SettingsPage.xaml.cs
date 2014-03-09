using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace HazeWP
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings isolatedStorageSettings;
        private bool isSelectionMade = false;

        public SettingsPage()
        {
            InitializeComponent();

            this.isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;
            this.SetPushNotificationTimePickers();
            this.LocationListPicker.ItemsSource = Location.locationCollection.Keys;
            this.LocationListPicker.SelectionChanged += LocationListPicker_SelectionChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string location;
            if (this.isolatedStorageSettings.TryGetValue<string>("Location", out location))
            {
                LocationListPicker.SelectedItem = location;
            }
            else
            {
                Button button = new Button();
                button.Content = "Check Haze API";
                button.BorderBrush = new SolidColorBrush(Colors.Black);
                button.Foreground = new SolidColorBrush(Colors.Black);
                button.Tap += button_Tap;
                this.ContentPanel.Children.Add(button);
            }

            bool isPushNotificationChecked;
            if (this.isolatedStorageSettings.TryGetValue<bool>("PushNotification", out isPushNotificationChecked))
            {
                this.PushNotificationToggleSwitch.IsChecked = isPushNotificationChecked;
                this.SetPushNotificationTimePickers();
            }
        }

        void button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.GoBack();
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

            this.isSelectionMade = true;
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask task = new MarketplaceReviewTask();
            task.Show();
        }

        private void EmailDevButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Regarding Haze MY for Windows Phone";
            task.To = "blty_2312@hotmail.my";
            task.Show();
        }

        private void PushNotificationToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            this.isolatedStorageSettings.Remove("PushNotification");
            this.isolatedStorageSettings.Add("PushNotification", PushNotificationToggleSwitch.IsChecked);
            this.isolatedStorageSettings.Save();
            this.SetPushNotificationTimePickers();
        }

        private void PushNotificationToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            this.isolatedStorageSettings.Remove("PushNotification");
            this.isolatedStorageSettings.Add("PushNotification", PushNotificationToggleSwitch.IsChecked);
            this.isolatedStorageSettings.Save();
            this.SetPushNotificationTimePickers();
        }

        #region Time Range for Push Notification

        private void SetPushNotificationTimePickers()
        {
            if ((bool) PushNotificationToggleSwitch.IsChecked)
            {
                this.StartTimePicker.Visibility = Visibility.Visible;
                this.EndTimePicker.Visibility = Visibility.Visible;
                
                DateTime startTime;
                if (this.isolatedStorageSettings.TryGetValue<DateTime>("PushNotificationStartTime", out startTime))
                {
                    this.StartTimePicker.Value = startTime;
                }
                
                DateTime endTime;
                if (this.isolatedStorageSettings.TryGetValue<DateTime>("PushNotificationEndTime", out endTime))
                {
                    this.EndTimePicker.Value = endTime;
                }
            }
            else
            {
                this.StartTimePicker.Visibility = Visibility.Collapsed;
                this.EndTimePicker.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        private void StartTimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            this.isolatedStorageSettings.Remove("PushNotificationStartTime");
            this.isolatedStorageSettings.Add("PushNotificationStartTime", StartTimePicker.Value);
        }

        private void EndTimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            this.isolatedStorageSettings.Remove("PushNotificationEndTime");
            this.isolatedStorageSettings.Add("PushNotificationEndTime", EndTimePicker.Value);
        }
    }
}