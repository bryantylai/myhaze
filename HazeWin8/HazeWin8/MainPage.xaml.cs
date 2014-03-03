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

            Win8AdRotator.PlatformAdProviderComponents.Add(AdRotator.Model.AdType.PubCenter, typeof(Microsoft.Advertising.WinRT.UI.AdControl));

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
                foreach (City city in state.Cities)
                {
                    count++;
                    RestClient restClient = new RestClient();
                    restClient.Get<City>(city.ID,
                        (result) =>
                        {
                            if (result != null)
                            {
                                CopyValue(city, result);
                            }
                        });
                }
            }
        }

        private void CopyValue(City city, City result)
        {
            city.Color = result.Color;
            city.Humidity = result.Humidity;
            city.ImageURL = result.ImageURL;
            city.Pressure = result.Pressure;
            city.PSI = result.PSI;
            city.Temperature = result.Temperature;
            city.TimeDiff = result.TimeDiff;
            if ((--count) == 0)
            {
                BindUI();
            }
        }

        private void InitializeStateCollection()
        {
            if (!(StateCollection.Count > 0))
            {
                State Johor = new State() { Name = "Johor", Cities = new ObservableCollection<City>() };
                Johor.Cities.Add(new City() { ID = "kota_tinggi", Location = "Kota Tinggi" });
                Johor.Cities.Add(new City() { ID = "larkin_lama", Location = "Larkin Lama" });
                Johor.Cities.Add(new City() { ID = "muar", Location = "Muar" });
                Johor.Cities.Add(new City() { ID = "pasir_gudang", Location = "Pasir Gudang" });

                State Kedah = new State() { Name = "Kedah", Cities = new ObservableCollection<City>() };
                Kedah.Cities.Add(new City() { ID = "alor_setar", Location = "Alor Setar" });
                Kedah.Cities.Add(new City() { ID = "bakar_arang", Location = "Bakar Arang, Sg. Petani" });
                Kedah.Cities.Add(new City() { ID = "langkawi", Location = "Langkawi" });

                State Kelantan = new State() { Name = "Kelantan", Cities = new ObservableCollection<City>() };
                Kelantan.Cities.Add(new City() { ID = "kota_bharu", Location = "Kota Bharu" });
                Kelantan.Cities.Add(new City() { ID = "tanah_merah", Location = "Tanah Merah" });

                State Melaka = new State() { Name = "Melaka", Cities = new ObservableCollection<City>() };
                Melaka.Cities.Add(new City() { ID = "bandaraya_melaka", Location = "Bandaraya Melaka" });
                Melaka.Cities.Add(new City() { ID = "bukit_rambai", Location = "Bukit Rambai" });

                State NSembilan = new State() { Name = "Negeri Sembilan", Cities = new ObservableCollection<City>() };
                NSembilan.Cities.Add(new City() { ID = "nilai", Location = "Nilai" });
                NSembilan.Cities.Add(new City() { ID = "port_dickson", Location = "Port Dickson" });
                NSembilan.Cities.Add(new City() { ID = "seremban", Location = "Seremban" });

                State Pahang = new State() { Name = "Pahang", Cities = new ObservableCollection<City>() };
                Pahang.Cities.Add(new City() { ID = "balok_baru", Location = "Balok Baru, Kuantan" });
                Pahang.Cities.Add(new City() { ID = "indera_mahkota", Location = "Indera Mahkota, Kuantan" });
                Pahang.Cities.Add(new City() { ID = "jerantut", Location = "Jerantut" });

                State Perak = new State() { Name = "Perak", Cities = new ObservableCollection<City>() };
                Perak.Cities.Add(new City() { ID = "jalan_tasek", Location = "Jalan Tasek, Ipoh" });
                Perak.Cities.Add(new City() { ID = "air_putih", Location = "Kg. Air Putih, Taiping" });
                Perak.Cities.Add(new City() { ID = "sk_jalan_pegoh", Location = "S K Jalan Pegoh, Ipoh" });
                Perak.Cities.Add(new City() { ID = "seri_manjung", Location = "Seri Manjung" });
                Perak.Cities.Add(new City() { ID = "tanjung_malim", Location = "Tanjung Malim" });

                State Perlis = new State() { Name = "Perlis", Cities = new ObservableCollection<City>() };
                Perlis.Cities.Add(new City() { ID = "kangar", Location = "Kangar" });

                State Pinang = new State() { Name = "Pulau Pinang", Cities = new ObservableCollection<City>() };
                Pinang.Cities.Add(new City() { ID = "perai", Location = "Perai" });
                Pinang.Cities.Add(new City() { ID = "seberang_jaya_2", Location = "Seberang Jaya 2" });
                Pinang.Cities.Add(new City() { ID = "usm", Location = "USM" });

                State Sabah = new State() { Name = "Sabah", Cities = new ObservableCollection<City>() };
                Sabah.Cities.Add(new City() { ID = "keningau", Location = "Keningau" });
                Sabah.Cities.Add(new City() { ID = "kota_kinabalu", Location = "Kota Kinabalu" });
                Sabah.Cities.Add(new City() { ID = "sandakan", Location = "Sandakan" });
                Sabah.Cities.Add(new City() { ID = "tawau", Location = "Tawau" });

                State Sarawak = new State() { Name = "Sarawak", Cities = new ObservableCollection<City>() };
                Sarawak.Cities.Add(new City() { ID = "bintulu", Location = "Bintulu" });
                Sarawak.Cities.Add(new City() { ID = "ilp_miri", Location = "ILP Miri" });
                Sarawak.Cities.Add(new City() { ID = "kapit", Location = "Kapit" });
                Sarawak.Cities.Add(new City() { ID = "kuching", Location = "Kuching" });
                Sarawak.Cities.Add(new City() { ID = "limbang", Location = "Limbang" });
                Sarawak.Cities.Add(new City() { ID = "miri", Location = "Miri" });
                Sarawak.Cities.Add(new City() { ID = "samarahan", Location = "Samarahan" });
                Sarawak.Cities.Add(new City() { ID = "sarikei", Location = "Sarikei" });
                Sarawak.Cities.Add(new City() { ID = "sibu", Location = "Sibu" });
                Sarawak.Cities.Add(new City() { ID = "sri_aman", Location = "Sri Aman" });

                State Selangor = new State() { Name = "Selangor", Cities = new ObservableCollection<City>() };
                Selangor.Cities.Add(new City() { ID = "banting", Location = "Banting" });
                Selangor.Cities.Add(new City() { ID = "kuala_selangor", Location = "Kuala Selangor" });
                Selangor.Cities.Add(new City() { ID = "pelabuhan_klang", Location = "Pelabuhan Klang" });
                Selangor.Cities.Add(new City() { ID = "petaling_jaya", Location = "(Puchong) Petaling Jaya" });
                Selangor.Cities.Add(new City() { ID = "shah_alam", Location = "Shah Alam" });

                State Terengganu = new State() { Name = "Terengganu", Cities = new ObservableCollection<City>() };
                Terengganu.Cities.Add(new City() { ID = "kemaman", Location = "Kemaman" });
                Terengganu.Cities.Add(new City() { ID = "kuala_terengganu", Location = "Kuala Terengganu" });

                State Wilayah = new State() { Name = "Wilayah Persekutuan", Cities = new ObservableCollection<City>() };
                Wilayah.Cities.Add(new City() { ID = "batu_muda", Location = "Batu Muda, Kuala Lumpur" });
                Wilayah.Cities.Add(new City() { ID = "cheras", Location = "Cheras, Kuala Lumpur" });
                Wilayah.Cities.Add(new City() { ID = "labuan", Location = "Labuan" });
                Wilayah.Cities.Add(new City() { ID = "putrajaya", Location = "Putrajaya" });

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
            collectionViewSource.ItemsPath = new PropertyPath("Cities");
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
            if (e.IsSourceZoomedInView == false)
            {
                HazeZoomedInGridView.SelectedItem = HazeZoomedOutGridView.SelectedItem = e.DestinationItem.Item = e.SourceItem.Item;
            }
        }

        private void HazeZoomedInGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HazeZoomedOutGridView.SelectedItem = HazeZoomedInGridView.SelectedItem = (City)e.ClickedItem;

            DataTransferManager.ShowShareUI();
        }

        async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            try
            {
                var request = args.Request;

                City c = (City)HazeZoomedInGridView.SelectedItem;

                request.Data.Properties.Title = "MY Haze";
                request.Data.SetText("Current API at " + c.Location + " is " + c.PSI + ". Updated " + c.TimeDiff + " via MY Haze #haze #myhaze");
                return;
            }
            catch (Exception)
            {
            }

            await new MessageDialog("You have not selected any API to share with.").ShowAsync();
        }

    }
}
