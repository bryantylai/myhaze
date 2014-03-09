using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Phone.Shell;

namespace HazeWP
{
    public class LiveTileUpdater
    {       
        public static void UpdateTile(string psinow, string psinowdate, SolidColorBrush brush, string location)
        {
            DynamicTile dynamicTile = new DynamicTile
            {
                API = psinow,
                Time = psinowdate,
                Location = location,
                Background = brush

            };

            // Retrieve the contents of the tile as a StandardTileData
            var newTile = dynamicTile.ToTile();

            // Use the new tile as the primary tile for this app.
            ShellTile primaryTile = ShellTile.ActiveTiles.First();

            if (primaryTile != null)
            {
                primaryTile.Update(newTile);
            }
        }
    }

    public class ToastUpdater
    {
        public static void UpdateToast(string api)
        {
            ShellToast toast = new ShellToast();
            toast.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
            toast.Title = "Haze MY";
            toast.Content = "Current API : " + api;
            toast.Show();
        }
    }
}
