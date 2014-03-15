using HazeWin8.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.ApplicationSettings;
using AdRotator;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace HazeWin8
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<State> StateCollection = new ObservableCollection<State>();
        private int count = 0;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();

            this.ResetAdvertisement();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;

            InitializeStateCollection();
            BindUI();
            UpdateCities();
        }

        void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand generalCommand = new SettingsCommand("DefaultsId", "Privacy Policy + Support", (handler) => new PrivacyFlyout().Show());
            args.Request.ApplicationCommands.Add(generalCommand);
        }

        private void UpdateCities()
        {
            foreach (State state in StateCollection)
            {
                foreach (Haze haze in state.HazeCollection)
                {
                    count++;
                    RestClient restClient = new RestClient();
                    restClient.Get<Haze>(haze.ID,
                        (result) =>
                        {
                            if (result != null)
                            {
                                CopyValue(haze, result);
                            }
                        });
                }
            }
        }

        private void CopyValue(Haze haze, Haze result)
        {
            haze.Color = result.Color;
            haze.PSI = result.PSI;
            haze.TimeDiff = result.TimeDiff;
            if ((--count) == 0)
            {
                BindUI();
            }
        }

        private void InitializeStateCollection()
        {
            if (!(StateCollection.Count > 0))
            {
                State Johor = new State() { Name = "Johor", HazeCollection = new ObservableCollection<Haze>() };
                Johor.HazeCollection.Add(new Haze() { ID = "kota_tinggi", Location = "Kota Tinggi" });
                Johor.HazeCollection.Add(new Haze() { ID = "larkin_lama", Location = "Larkin Lama" });
                Johor.HazeCollection.Add(new Haze() { ID = "muar", Location = "Muar" });
                Johor.HazeCollection.Add(new Haze() { ID = "pasir_gudang", Location = "Pasir Gudang" });

                State Kedah = new State() { Name = "Kedah", HazeCollection = new ObservableCollection<Haze>() };
                Kedah.HazeCollection.Add(new Haze() { ID = "alor_setar", Location = "Alor Setar" });
                Kedah.HazeCollection.Add(new Haze() { ID = "bakar_arang", Location = "Bakar Arang, Sg. Petani" });
                Kedah.HazeCollection.Add(new Haze() { ID = "langkawi", Location = "Langkawi" });

                State Kelantan = new State() { Name = "Kelantan", HazeCollection = new ObservableCollection<Haze>() };
                Kelantan.HazeCollection.Add(new Haze() { ID = "kota_bharu", Location = "Kota Bharu" });
                Kelantan.HazeCollection.Add(new Haze() { ID = "tanah_merah", Location = "Tanah Merah" });

                State Melaka = new State() { Name = "Melaka", HazeCollection = new ObservableCollection<Haze>() };
                Melaka.HazeCollection.Add(new Haze() { ID = "bandaraya_melaka", Location = "Bandaraya Melaka" });
                Melaka.HazeCollection.Add(new Haze() { ID = "bukit_rambai", Location = "Bukit Rambai" });

                State NSembilan = new State() { Name = "Negeri Sembilan", HazeCollection = new ObservableCollection<Haze>() };
                NSembilan.HazeCollection.Add(new Haze() { ID = "nilai", Location = "Nilai" });
                NSembilan.HazeCollection.Add(new Haze() { ID = "port_dickson", Location = "Port Dickson" });
                NSembilan.HazeCollection.Add(new Haze() { ID = "seremban", Location = "Seremban" });

                State Pahang = new State() { Name = "Pahang", HazeCollection = new ObservableCollection<Haze>() };
                Pahang.HazeCollection.Add(new Haze() { ID = "balok_baru", Location = "Balok Baru, Kuantan" });
                Pahang.HazeCollection.Add(new Haze() { ID = "indera_mahkota", Location = "Indera Mahkota, Kuantan" });
                Pahang.HazeCollection.Add(new Haze() { ID = "jerantut", Location = "Jerantut" });

                State Perak = new State() { Name = "Perak", HazeCollection = new ObservableCollection<Haze>() };
                Perak.HazeCollection.Add(new Haze() { ID = "jalan_tasek", Location = "Jalan Tasek, Ipoh" });
                Perak.HazeCollection.Add(new Haze() { ID = "air_putih", Location = "Kg. Air Putih, Taiping" });
                Perak.HazeCollection.Add(new Haze() { ID = "sk_jalan_pegoh", Location = "S K Jalan Pegoh, Ipoh" });
                Perak.HazeCollection.Add(new Haze() { ID = "seri_manjung", Location = "Seri Manjung" });
                Perak.HazeCollection.Add(new Haze() { ID = "tanjung_malim", Location = "Tanjung Malim" });

                State Perlis = new State() { Name = "Perlis", HazeCollection = new ObservableCollection<Haze>() };
                Perlis.HazeCollection.Add(new Haze() { ID = "kangar", Location = "Kangar" });

                State Pinang = new State() { Name = "Pulau Pinang", HazeCollection = new ObservableCollection<Haze>() };
                Pinang.HazeCollection.Add(new Haze() { ID = "perai", Location = "Perai" });
                Pinang.HazeCollection.Add(new Haze() { ID = "seberang_jaya_2", Location = "Seberang Jaya 2" });
                Pinang.HazeCollection.Add(new Haze() { ID = "usm", Location = "USM" });

                State Sabah = new State() { Name = "Sabah", HazeCollection = new ObservableCollection<Haze>() };
                Sabah.HazeCollection.Add(new Haze() { ID = "keningau", Location = "Keningau" });
                Sabah.HazeCollection.Add(new Haze() { ID = "kota_kinabalu", Location = "Kota Kinabalu" });
                Sabah.HazeCollection.Add(new Haze() { ID = "sandakan", Location = "Sandakan" });
                Sabah.HazeCollection.Add(new Haze() { ID = "tawau", Location = "Tawau" });

                State Sarawak = new State() { Name = "Sarawak", HazeCollection = new ObservableCollection<Haze>() };
                Sarawak.HazeCollection.Add(new Haze() { ID = "bintulu", Location = "Bintulu" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "ilp_miri", Location = "ILP Miri" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "kapit", Location = "Kapit" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "kuching", Location = "Kuching" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "limbang", Location = "Limbang" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "miri", Location = "Miri" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "samarahan", Location = "Samarahan" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "sarikei", Location = "Sarikei" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "sibu", Location = "Sibu" });
                Sarawak.HazeCollection.Add(new Haze() { ID = "sri_aman", Location = "Sri Aman" });

                State Selangor = new State() { Name = "Selangor", HazeCollection = new ObservableCollection<Haze>() };
                Selangor.HazeCollection.Add(new Haze() { ID = "banting", Location = "Banting" });
                Selangor.HazeCollection.Add(new Haze() { ID = "kuala_selangor", Location = "Kuala Selangor" });
                Selangor.HazeCollection.Add(new Haze() { ID = "pelabuhan_klang", Location = "Pelabuhan Klang" });
                Selangor.HazeCollection.Add(new Haze() { ID = "petaling_jaya", Location = "(Puchong) Petaling Jaya" });
                Selangor.HazeCollection.Add(new Haze() { ID = "shah_alam", Location = "Shah Alam" });

                State Terengganu = new State() { Name = "Terengganu", HazeCollection = new ObservableCollection<Haze>() };
                Terengganu.HazeCollection.Add(new Haze() { ID = "kemaman", Location = "Kemaman" });
                Terengganu.HazeCollection.Add(new Haze() { ID = "kuala_terengganu", Location = "Kuala Terengganu" });

                State Wilayah = new State() { Name = "Wilayah Persekutuan", HazeCollection = new ObservableCollection<Haze>() };
                Wilayah.HazeCollection.Add(new Haze() { ID = "batu_muda", Location = "Batu Muda, Kuala Lumpur" });
                Wilayah.HazeCollection.Add(new Haze() { ID = "cheras", Location = "Cheras, Kuala Lumpur" });
                Wilayah.HazeCollection.Add(new Haze() { ID = "labuan", Location = "Labuan" });
                Wilayah.HazeCollection.Add(new Haze() { ID = "putrajaya", Location = "Putrajaya" });

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
            }
        }

        private void BindUI()
        {
            collectionViewSource.Source = StateCollection;
            collectionViewSource.ItemsPath = new PropertyPath("HazeCollection");
            HazeZoomedInGridView.SelectedItem = null;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void HazeSemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            this.ResetAdvertisement();
            if (e.IsSourceZoomedInView == false)
            {
                HazeZoomedInGridView.SelectedItem = HazeZoomedOutGridView.SelectedItem = e.DestinationItem.Item = e.SourceItem.Item;
                backButton.Visibility = Visibility.Visible;
            }
            else
            {
                HazeZoomedInGridView.SelectedItem = HazeZoomedOutGridView.SelectedItem = e.DestinationItem.Item = e.SourceItem.Item = null;
                backButton.Visibility = Visibility.Collapsed;
            }
        }

        private void ResetAdvertisement()
        {
            AdvertisementGrid.Children.Clear();
            AdRotatorControl adRotator = new AdRotatorControl();
            adRotator.AdHeight = 90; adRotator.Height = 90;
            adRotator.AdWidth = 728; adRotator.Width = 728;
            adRotator.LocalSettingsLocation = "defaultAdSettings.xml";
            adRotator.AutoStartAds = true;
            adRotator.PlatformAdProviderComponents.Add(AdRotator.Model.AdType.PubCenter, typeof(Microsoft.Advertising.WinRT.UI.AdControl));
            AdvertisementGrid.Children.Add(adRotator);
        }

        private void HazeZoomedInGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HazeZoomedOutGridView.SelectedItem = HazeZoomedInGridView.SelectedItem = (Haze)e.ClickedItem;

            DataTransferManager.ShowShareUI();
        }

        async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            try
            {
                var request = args.Request;

                Haze haze = (Haze)HazeZoomedInGridView.SelectedItem;

                request.Data.Properties.Title = "Haze MY";
                request.Data.SetText("Current API at " + haze.Location + " is " + haze.PSI + ". Updated " + haze.TimeDiff + " via Haze MY #haze #myhaze http://aka.ms/hazemywin8");
                return;
            }
            catch (Exception)
            {
            }

            await new MessageDialog("You have not selected any API to share with.").ShowAsync();
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (backButton.Visibility == Visibility.Visible)
            {
                HazeSemanticZoom.IsZoomedInViewActive = false;
            }
        }

        private void RefreshAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateCities();
            BottomApplicationBar.IsOpen = false;
        }
    }
}
