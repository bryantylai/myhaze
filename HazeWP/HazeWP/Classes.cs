using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazeWP
{
    public class HazeWithHistory
    {
        public Haze Haze { get; set; }
        public LinkedList<History> Histories { get; set; }
    }

    public class Haze
    {
        public string ID { get; set; }
        public string Location { get; set; }
        public string PSI { get; set; }
        public string TimeDiff { get; set; }
        public string Color { get; set; }
    }

    public class History
    {
        public string PSI { get; set; }
        public string PSIDiff { get; set; }
        public string Color { get; set; }
        public string ColorDiff { get; set; }
        public string TimeDiff { get; set; }
    }

    public class Location
    {
        public static Dictionary<string, string> locationCollection = new Dictionary<string, string>()
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
                { "Selangor, Pelabuhan Klang", "pelabuhan_kelang" },
                { "Selangor, (Puchong) Petaling Jaya", "petaling_jaya" },
                { "Selangor, Shah Alam", "shah_alam" },
                { "Terengganu, Kemaman", "kemaman" },
                { "Terengganu, Kuala Terengganu", "kuala_terengganu" },
                { "Wilayah Persekutuan, Labuan", "labuan" },
                { "Wilayah Persekutuan, Putrajaya", "putrajaya" },
            };
        
    }
}
