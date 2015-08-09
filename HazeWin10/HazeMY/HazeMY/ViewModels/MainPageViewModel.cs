using HazeMY.Models;
using HazeMY.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.UI.Popups;

namespace HazeMY.ViewModels
{
    public class MainPageViewModel : BindableObjectBase
    {
        public MainPageViewModel()
        {
            InitializeLocations();
            
            if (ApplicationData.Current.LocalSettings.Values["SelectedLocation"] != null)
            {
                SelectedLocation = ApplicationData.Current.LocalSettings.Values["SelectedLocation"].ToString();
            }
            else
            {
                SelectedLocation = Locations.Select(l => l.Key).FirstOrDefault();
            }
        }

        private void InitializeLocations()
        {
            Locations = new Dictionary<string, string>()
            {
                { "Johor, Kota Tinggi", "kota_tinggi" },
                { "Johor, Larkin Lama", "larkin_lama" },
                { "Johor, Muar", "muar" },
                { "Johor, Pasir Gudang", "pasir_gudang" },
                { "Kedah, Alor Setar", "alor_setar" },
                { "Kedah, Bakar Arang, Sg. Petani", "bakar_arang" },
                { "Kedah, Langkawi", "langkawi" },
                { "Kelantan, Kota Bharu", "kota_bharu" },
                { "Kelantan, Tanah Merah", "tanah_merah" },
                { "Kuala Lumpur, Batu Muda", "batu_muda" },
                { "Kuala Lumpur, Cheras", "cheras" },
                { "Melaka, Bandaraya Melaka", "bandaraya_melaka" },
                { "Melaka, Bukit Rambai", "bukit_rambai" },
                { "Negeri Sembilan, Nilai", "nilai" },
                { "Negeri Sembilan, Port Dickson", "port_dickson" },
                { "Negeri Sembilan, Seremban", "seremban" },
                { "Pahang, Balok Baru, Kuantan", "balok_baru" },
                { "Pahang, Indera Mahkota, Kuantan", "indera_mahkota" },
                { "Pahang, Jerantut", "jerantut" },
                { "Perak, Jalan Tasek, Ipoh", "jalan_tasek" },
                { "Perak, Kg. Air Putih, Taiping", "air_putih" },
                { "Perak, S K Jalan Pegoh, Ipoh", "sk_jalan_pegoh" },
                { "Perak, Seri Manjung", "seri_manjung" },
                { "Perak, Tanjung Malim", "tanjung_malim" },
                { "Perlis, Kangar", "kangar" },
                { "Pulau Pinang, Perai", "perai" },
                { "Pulau Pinang, Seberang Jaya 2, Perai", "seberang_jaya_2" },
                { "Pulau Pinang, USM", "usm" },
                { "Sabah, Keningau", "keningau" },
                { "Sabah, Kota Kinabalu", "kota_kinabalu" },
                { "Sabah, Sandakan", "sandakan" },
                { "Sabah, Tawau", "tawau" },
                { "Sarawak, Bintulu", "bintulu" },
                { "Sarawak, ILP Miri", "ilp_miri" },
                { "Sarawak, Kapit", "kapit" },
                { "Sarawak, Kuching", "kuching" },
                { "Sarawak, Limbang", "limbang" },
                { "Sarawak, Miri", "miri" },
                { "Sarawak, Samarahan", "samarahan" },
                { "Sarawak, Sarikei", "sarikei" },
                { "Sarawak, Sibu", "sibu" },
                { "Sarawak, Sri Aman", "sri_aman" },
                { "Selangor, Banting", "banting" },
                { "Selangor, Kuala Selangor", "kuala_selangor" },
                { "Selangor, Pelabuhan Klang", "pelabuhan_klang" },
                { "Selangor, (Puchong) Petaling Jaya", "petaling_jaya" },
                { "Selangor, Shah Alam", "shah_alam" },
                { "Terengganu, Kemaman", "kemaman" },
                { "Terengganu, Kuala Terengganu", "kuala_terengganu" },
                { "Wilayah Persekutuan, Labuan", "labuan" },
                { "Wilayah Persekutuan, Putrajaya", "putrajaya" },
            };
        }

        private IHazeService _hazeService;
        public IHazeService HazeService
        {
            get
            {
                return _hazeService ?? (_hazeService = new HazeService());
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged(() => IsBusy);
            }
        }

        private Dictionary<string, string> _locations;
        public Dictionary<string, string> Locations
        {
            get
            {
                return _locations;
            }

            set
            {
                _locations = value;
                OnPropertyChanged(() => Locations);
                OnPropertyChanged(() => LocationListDisplay);
            }
        }

        public List<string> LocationListDisplay
        {
            get
            {
                return Locations.Keys.ToList<string>();
            }
        }

        private string _prevSelectedLocation;
        private string _selectedLocation;
        public string SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }

            set
            {
                if (value == _selectedLocation) return;

                _prevSelectedLocation = _selectedLocation;
                _selectedLocation = value;

                GetData();

                OnPropertyChanged(() => SelectedLocation);
            }
        }

        private HazeWithHistory _hazeWithHistory;
        public HazeWithHistory HazeWithHistory
        {
            get
            {
                return _hazeWithHistory;
            }
            set
            {
                _hazeWithHistory = value;
                OnPropertyChanged(() => HazeWithHistory);

            }
        }

        private async void GetData()
        {
            Exception exception = null;

            try
            {
                string selectedLocationValue = string.Empty;
                if (Locations.TryGetValue(SelectedLocation, out selectedLocationValue))
                {
                    IsBusy = true;
                    HazeWithHistoryContainer hazeWithHistoryContainer = await HazeService.GetByLocationId(selectedLocationValue);
                    if (hazeWithHistoryContainer.HazeWithHistory == null)
                    {
                        await ShowErrorDialog(hazeWithHistoryContainer.Exception.Message, hazeWithHistoryContainer.Exception.StackTrace);
                    }
                    else
                    {
                        HazeWithHistory = hazeWithHistoryContainer.HazeWithHistory;
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                IsBusy = false;

                if (exception != null)
                {
                    _selectedLocation = _prevSelectedLocation;
                    OnPropertyChanged(() => SelectedLocation);

                    await ShowErrorDialog(exception.Message, exception.StackTrace);
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["SelectedLocation"] = SelectedLocation;
                }
            }
        }

        private static async Task ShowErrorDialog(string message, string stackTrace)
        {
            MessageDialog messageDialog = new MessageDialog(message, "An Error Has Occured");
            messageDialog.Commands.Add(new UICommand("Report", async (action) =>
            {
                EmailMessage emailMsg = new EmailMessage();
                emailMsg.Subject = "Haze MY Application Error";
                emailMsg.Body = message + "\r\n" + stackTrace;
                emailMsg.To.Add(new EmailRecipient("bryantylai_app_dev@outlook.com"));

                await EmailManager.ShowComposeNewEmailAsync(emailMsg);
            }));
            messageDialog.Commands.Add(new UICommand("Cancel"));
            await messageDialog.ShowAsync();
        }
    }
}
